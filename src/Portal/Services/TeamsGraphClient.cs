using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Identity.Web;

namespace PartnerAppPortal.Portal.Services;

public sealed class TeamsGraphClient
{
    private static readonly string[] DefaultScopes =
    {
        "https://graph.microsoft.com/Team.ReadBasic.All",
        "https://graph.microsoft.com/Channel.ReadBasic.All",
        "https://graph.microsoft.com/ChannelMessage.Read.All",
        "https://graph.microsoft.com/ChannelMessage.Send",
        "https://graph.microsoft.com/Files.Read.All"
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public TeamsGraphClient(
        IHttpClientFactory httpClientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<TeamsChannelRef> FindAccountChannelAsync(string accountName, ClaimsPrincipal user)
    {
        var teams = await GetAsync<GraphList<GraphTeam>>("me/joinedTeams", user);
        var team = teams.Value.FirstOrDefault(t =>
            string.Equals(t.DisplayName?.Trim(), accountName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (team?.Id == null)
            throw new InvalidOperationException(
                $"No Team named \"{accountName}\" was found for this sign-in. The Team is created at onboarding and the partner must already be a member.");

        var channels = await GetAsync<GraphList<GraphChannel>>($"teams/{team.Id}/channels", user);
        var channel = channels.Value.FirstOrDefault(c =>
                string.Equals(c.DisplayName, "General", StringComparison.OrdinalIgnoreCase))
            ?? channels.Value.FirstOrDefault();
        if (channel?.Id == null)
            throw new InvalidOperationException($"Team \"{accountName}\" has no channel.");

        return new TeamsChannelRef(team.Id, channel.Id, team.DisplayName ?? accountName, channel.DisplayName ?? "General");
    }

    public async Task<IReadOnlyList<TeamsChatLine>> GetRecentMessagesAsync(string teamId, string channelId, ClaimsPrincipal user)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-30);
        var lines = new List<TeamsChatLine>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var parentsNeedingReplies = new List<string>();
        var url = $"teams/{teamId}/channels/{channelId}/messages?$top=50";

        // Walk newest → older until we pass the 30-day window (or run out of pages).
        for (var page = 0; page < 20 && !string.IsNullOrWhiteSpace(url); page++)
        {
            var batch = await GetAsync<GraphList<GraphMessage>>(url, user);
            var reachedCutoff = false;
            foreach (var message in batch.Value)
            {
                if (message.CreatedDateTime < cutoff)
                {
                    reachedCutoff = true;
                    continue;
                }

                if (message.MessageType is "systemEventMessage")
                    continue;

                AddMessageLine(lines, seen, message);

                // Channel message list does not include thread replies.
                if (!string.IsNullOrWhiteSpace(message.Id) &&
                    (message.ReplyCount > 0 || parentsNeedingReplies.Count < 25))
                {
                    parentsNeedingReplies.Add(message.Id);
                }
            }

            if (reachedCutoff)
                break;

            url = ToRelative(batch.NextLink);
        }

        foreach (var parentId in parentsNeedingReplies.Distinct(StringComparer.Ordinal))
            await AddRepliesAsync(teamId, channelId, parentId, cutoff, lines, seen, user);

        lines.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
        return lines;
    }

    private async Task AddRepliesAsync(
        string teamId,
        string channelId,
        string messageId,
        DateTimeOffset cutoff,
        List<TeamsChatLine> lines,
        HashSet<string> seen,
        ClaimsPrincipal user)
    {
        try
        {
            var url = $"teams/{teamId}/channels/{channelId}/messages/{messageId}/replies?$top=50";
            for (var page = 0; page < 10 && !string.IsNullOrWhiteSpace(url); page++)
            {
                var batch = await GetAsync<GraphList<GraphMessage>>(url, user);
                foreach (var reply in batch.Value)
                {
                    if (reply.CreatedDateTime < cutoff)
                        continue;
                    if (reply.MessageType is "systemEventMessage")
                        continue;
                    AddMessageLine(lines, seen, reply);
                }

                url = ToRelative(batch.NextLink);
            }
        }
        catch (InvalidOperationException)
        {
            // Keep parent messages even if a replies call fails.
        }
    }

    private static void AddMessageLine(List<TeamsChatLine> lines, HashSet<string> seen, GraphMessage message)
    {
        var text = ToText(message.Body?.Content);
        if (string.IsNullOrWhiteSpace(text))
            return;

        var id = message.Id ?? Guid.NewGuid().ToString("N");
        if (!seen.Add(id))
            return;

        lines.Add(new TeamsChatLine(
            id,
            message.From?.User?.DisplayName ?? "Teams",
            message.CreatedDateTime,
            text));
    }

    public async Task<TeamsChatLine> SendAsync(string teamId, string channelId, string text, ClaimsPrincipal user)
    {
        var payload = new
        {
            body = new
            {
                contentType = "html",
                content = System.Net.WebUtility.HtmlEncode(text.Trim())
            }
        };
        var message = await PostAsync<GraphMessage>($"teams/{teamId}/channels/{channelId}/messages", payload, user);
        return new TeamsChatLine(
            message.Id ?? Guid.NewGuid().ToString("N"),
            message.From?.User?.DisplayName ?? "You",
            message.CreatedDateTime == default ? DateTimeOffset.UtcNow : message.CreatedDateTime,
            text.Trim());
    }

    public async Task<TeamsFolderListing> ListAccountFilesAsync(
        string accountName,
        ClaimsPrincipal user,
        string? folderItemId = null,
        string? libraryRootItemId = null)
    {
        var channel = await FindAccountChannelAsync(accountName, user);
        var root = await GetAsync<GraphDriveItem>(
            $"teams/{channel.TeamId}/channels/{channel.ChannelId}/filesFolder",
            user);

        var driveId = root.ParentReference?.DriveId;
        if (string.IsNullOrWhiteSpace(driveId))
            throw new InvalidOperationException("This Teams channel does not expose a document library yet.");

        var libraryRoot = string.IsNullOrWhiteSpace(libraryRootItemId) ? root.Id : libraryRootItemId;
        var itemId = string.IsNullOrWhiteSpace(folderItemId) ? libraryRoot : folderItemId;
        if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(libraryRoot))
            throw new InvalidOperationException("The channel Files folder could not be resolved.");

        var folder = string.Equals(itemId, root.Id, StringComparison.Ordinal)
            ? root
            : await GetAsync<GraphDriveItem>($"drives/{driveId}/items/{itemId}", user);

        var children = await GetAsync<GraphList<GraphDriveItem>>(
            $"drives/{driveId}/items/{itemId}/children?$orderby=name",
            user);

        if (IsHiddenLibraryFolder(folder.Name) &&
            !string.IsNullOrWhiteSpace(folderItemId) &&
            !string.Equals(itemId, root.Id, StringComparison.Ordinal))
        {
            return await ListAccountFilesAsync(accountName, user, null);
        }

        var items = children.Value
            .Select(x => new TeamsDriveItem(
                x.Id ?? "",
                x.Name ?? "Untitled",
                x.Folder is not null,
                x.Size ?? 0,
                x.LastModifiedDateTime,
                x.DownloadUrl,
                x.WebUrl,
                driveId,
                x.File?.MimeType))
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .Where(x => !x.IsFolder || !IsHiddenLibraryFolder(x.Name))
            .OrderByDescending(x => x.IsFolder)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new TeamsFolderListing(
            channel.TeamName,
            channel.ChannelName,
            driveId,
            itemId,
            folder.Name ?? channel.ChannelName,
            string.Equals(itemId, libraryRoot, StringComparison.Ordinal),
            items);
    }

    public async Task<TeamsDriveItem?> FindAccountFolderByPathAsync(
        string accountName,
        string relativePath,
        ClaimsPrincipal user)
    {
        var parts = (relativePath ?? "")
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Any(p => p.Contains("..", StringComparison.Ordinal) || p.Contains(':')))
            return null;

        var folder = await GetGeneralFilesFolderAsync(accountName, user);
        var driveId = folder.DriveId;
        var parentId = folder.ItemId;
        GraphDriveItem? current = null;

        foreach (var part in parts)
        {
            current = await FindChildAsync(driveId, parentId, part, user);
            if (current?.Id is null || current.Folder is null)
                return null;
            parentId = current.Id;
        }

        if (current is null || string.IsNullOrWhiteSpace(current.Id))
            return null;

        return new TeamsDriveItem(
            current.Id,
            current.Name ?? parts.Last(),
            true,
            current.Size ?? 0,
            current.LastModifiedDateTime,
            current.DownloadUrl,
            current.WebUrl,
            driveId,
            null);
    }

    public async Task<TeamsDriveItem?> FindAccountFileByPathAsync(
        string accountName,
        string relativePath,
        ClaimsPrincipal user)
    {
        var parts = (relativePath ?? "")
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Any(p => p.Contains("..", StringComparison.Ordinal) || p.Contains(':')))
            return null;

        var folder = await GetGeneralFilesFolderAsync(accountName, user);
        var driveId = folder.DriveId;
        var parentId = folder.ItemId;
        GraphDriveItem? current = null;

        foreach (var part in parts)
        {
            current = await FindChildAsync(driveId, parentId, part, user);
            if (current?.Id is null)
                return null;
            parentId = current.Id;
        }

        if (current is null || current.Folder is not null || string.IsNullOrWhiteSpace(current.Id))
            return null;

        return new TeamsDriveItem(
            current.Id,
            current.Name ?? parts.Last(),
            false,
            current.Size ?? 0,
            current.LastModifiedDateTime,
            current.DownloadUrl,
            current.WebUrl,
            driveId,
            current.File?.MimeType);
    }

    public async Task<TeamsFileBytes?> DownloadAccountFileByPathAsync(
        string accountName,
        string relativePath,
        ClaimsPrincipal user)
    {
        var item = await FindAccountFileByPathAsync(accountName, relativePath, user);
        if (item is null)
            return null;

        var downloadUrl = item.DownloadUrl;
        if (string.IsNullOrWhiteSpace(downloadUrl))
            downloadUrl = await GetAccountFileDownloadUrlAsync(accountName, item.DriveId, item.Id, user);

        var client = _httpClientFactory.CreateClient("Graph");
        using var download = await client.GetAsync(downloadUrl);
        if (!download.IsSuccessStatusCode)
        {
            var body = await download.Content.ReadAsStringAsync();
            throw new InvalidOperationException(GraphError(body, (int)download.StatusCode));
        }

        var bytes = await download.Content.ReadAsByteArrayAsync();
        var contentType = download.Content.Headers.ContentType?.ToString()
            ?? item.MimeType
            ?? SiteReportFiles.ContentTypeFor(item.Name, null);
        return new TeamsFileBytes(item.Name, contentType, bytes);
    }

    public async Task<string> GetAccountFileDownloadUrlAsync(
        string accountName,
        string driveId,
        string itemId,
        ClaimsPrincipal user)
    {
        // Ensure the caller is scoped to this partner's Team before handing out a URL.
        _ = await FindAccountChannelAsync(accountName, user);

        // %40 = @ ; required so Graph returns the pre-authenticated download URL.
        var meta = await GetAsync<GraphDriveItem>(
            $"drives/{driveId}/items/{itemId}?$select=id,name,file,folder,%40microsoft.graph.downloadUrl",
            user);
        if (meta.Folder is not null)
            throw new InvalidOperationException("That item is a folder, not a file.");

        if (!string.IsNullOrWhiteSpace(meta.DownloadUrl))
            return meta.DownloadUrl;

        // Fallback: ask Graph for /content without following redirects, then use Location.
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"drives/{driveId}/items/{itemId}/content");
        using var response = await SendRawAsync(request, user, allowAutoRedirect: false);
        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrWhiteSpace(location) && response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                "Graph returned file bytes instead of a download URL. Try Connect Teams files again.");
        if (string.IsNullOrWhiteSpace(location))
            throw new InvalidOperationException("Microsoft Graph did not return a download URL for that file.");

        return location;
    }

    private static bool IsHiddenLibraryFolder(string? name) =>
        string.Equals(name?.Trim(), "Portal Artifacts", StringComparison.OrdinalIgnoreCase);

    private async Task<(string DriveId, string ItemId)> GetGeneralFilesFolderAsync(
        string accountName,
        ClaimsPrincipal user)
    {
        var channel = await FindAccountChannelAsync(accountName, user);
        var root = await GetAsync<GraphDriveItem>(
            $"teams/{channel.TeamId}/channels/{channel.ChannelId}/filesFolder",
            user);
        var driveId = root.ParentReference?.DriveId;
        if (string.IsNullOrWhiteSpace(driveId) || string.IsNullOrWhiteSpace(root.Id))
            throw new InvalidOperationException("This Teams channel does not expose a document library yet.");

        return (driveId, root.Id);
    }

    private async Task<GraphDriveItem?> FindChildAsync(
        string driveId,
        string parentId,
        string name,
        ClaimsPrincipal user)
    {
        string? url =
            $"drives/{driveId}/items/{parentId}/children?$select=id,name,size,file,folder,%40microsoft.graph.downloadUrl,webUrl,parentReference&$top=200";

        while (!string.IsNullOrWhiteSpace(url))
        {
            var batch = await GetAsync<GraphList<GraphDriveItem>>(url, user);
            var match = batch.Value.FirstOrDefault(x =>
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
                return match;

            url = ToRelative(batch.NextLink);
        }

        return null;
    }

    private async Task<T> GetAsync<T>(string relativeUrl, ClaimsPrincipal user)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        return await SendAsync<T>(request, user);
    }

    private async Task<T> PostAsync<T>(string relativeUrl, object body, ClaimsPrincipal user)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl)
        {
            Content = JsonContent.Create(body)
        };
        return await SendAsync<T>(request, user);
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("Sign in before opening Teams chat.");

        // Blazor AuthenticationState principals break MSAL silent acquire
        // ("No account or login hint was passed..."). Let Identity.Web resolve
        // the signed-in user from the request HttpContext, same as Connect Teams.
        var tenantId = _configuration["Graph:ResourceTenantId"];
        if (string.IsNullOrWhiteSpace(tenantId) || tenantId.Contains("YOUR_", StringComparison.Ordinal))
        {
            tenantId = user.FindFirst("tid")?.Value
                ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        }

        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
            Scopes(),
            tenantId: string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim(),
            tokenAcquisitionOptions: request.Method == HttpMethod.Post
                ? new TokenAcquisitionOptions { ForceRefresh = true }
                : null);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var client = _httpClientFactory.CreateClient("Graph");
        using var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(GraphError(body, (int)response.StatusCode));

        return System.Text.Json.JsonSerializer.Deserialize<T>(body, JsonOptions)
            ?? throw new InvalidOperationException("Microsoft Graph returned an empty response.");
    }

    private async Task<HttpResponseMessage> SendRawAsync(
        HttpRequestMessage request,
        ClaimsPrincipal user,
        bool allowAutoRedirect = true)
    {
        if (user.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("Sign in before opening Teams files.");

        var tenantId = _configuration["Graph:ResourceTenantId"];
        if (string.IsNullOrWhiteSpace(tenantId) || tenantId.Contains("YOUR_", StringComparison.Ordinal))
        {
            tenantId = user.FindFirst("tid")?.Value
                ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        }

        var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
            Scopes(),
            tenantId: string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim());

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var clientName = allowAutoRedirect ? "Graph" : "GraphNoRedirect";
        var client = _httpClientFactory.CreateClient(clientName);
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (allowAutoRedirect)
        {
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var status = (int)response.StatusCode;
                response.Dispose();
                throw new InvalidOperationException(GraphError(body, status));
            }

            return response;
        }

        // 302/301 to the pre-authenticated SharePoint URL is success for download resolution.
        if ((int)response.StatusCode is >= 300 and < 400 && response.Headers.Location is not null)
            return response;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;
            response.Dispose();
            throw new InvalidOperationException(GraphError(body, status));
        }

        return response;
    }

    private string[] Scopes()
    {
        var configured = _configuration["Graph:Scopes"];
        if (string.IsNullOrWhiteSpace(configured))
            return DefaultScopes;

        var scopes = configured.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return scopes.Length == 0 ? DefaultScopes : scopes;
    }

    private static string? ToRelative(string? nextLink)
    {
        if (string.IsNullOrWhiteSpace(nextLink))
            return null;
        const string prefix = "https://graph.microsoft.com/v1.0/";
        return nextLink.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? nextLink[prefix.Length..]
            : nextLink;
    }

    private static string GraphError(string body, int status)
    {
        try
        {
            var error = System.Text.Json.JsonSerializer.Deserialize<GraphErrorEnvelope>(body, JsonOptions);
            if (!string.IsNullOrWhiteSpace(error?.Error?.Message))
                return error.Error.Message;
        }
        catch
        {
            // Fall through to the status code.
        }

        return $"Microsoft Graph returned {status}.";
    }

    private static string ToText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return "";
        var stripped = Regex.Replace(html, "<[^>]+>", " ");
        return System.Net.WebUtility.HtmlDecode(stripped).Replace('\u00a0', ' ').Trim();
    }

    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class GraphList<T>
    {
        public List<T> Value { get; set; } = new();

        [JsonPropertyName("@odata.nextLink")]
        public string? NextLink { get; set; }
    }

    private sealed class GraphTeam
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
    }

    private sealed class GraphChannel
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
    }

    private sealed class GraphMessage
    {
        public string? Id { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? MessageType { get; set; }
        public int ReplyCount { get; set; }
        public GraphBody? Body { get; set; }
        public GraphFrom? From { get; set; }
    }

    private sealed class GraphDriveItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public long? Size { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
        public string? WebUrl { get; set; }
        public GraphFolderFacet? Folder { get; set; }
        public GraphFileFacet? File { get; set; }
        public GraphItemReference? ParentReference { get; set; }

        [JsonPropertyName("@microsoft.graph.downloadUrl")]
        public string? DownloadUrl { get; set; }
    }

    private sealed class GraphFolderFacet;

    private sealed class GraphFileFacet
    {
        public string? MimeType { get; set; }
    }

    private sealed class GraphItemReference
    {
        public string? DriveId { get; set; }
        public string? Id { get; set; }
    }

    private sealed class GraphBody
    {
        public string? Content { get; set; }
    }

    private sealed class GraphFrom
    {
        public GraphUser? User { get; set; }
    }

    private sealed class GraphUser
    {
        public string? DisplayName { get; set; }
    }

    private sealed class GraphErrorEnvelope
    {
        public GraphErrorBody? Error { get; set; }
    }

    private sealed class GraphErrorBody
    {
        public string? Message { get; set; }
    }
}

public sealed record TeamsChannelRef(string TeamId, string ChannelId, string TeamName, string ChannelName);

public sealed record TeamsChatLine(string Id, string Author, DateTimeOffset CreatedAt, string Text);

public sealed record TeamsDriveItem(
    string Id,
    string Name,
    bool IsFolder,
    long Size,
    DateTimeOffset? LastModified,
    string? DownloadUrl,
    string? WebUrl,
    string DriveId,
    string? MimeType);

public sealed record TeamsFolderListing(
    string TeamName,
    string ChannelName,
    string DriveId,
    string FolderItemId,
    string FolderName,
    bool IsRoot,
    IReadOnlyList<TeamsDriveItem> Items);

public sealed record TeamsFileBytes(string FileName, string ContentType, byte[] Bytes);

