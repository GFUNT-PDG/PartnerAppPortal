using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using PartnerAppPortal.Core.Models.Onboarding;

namespace PartnerAppPortal.Api.Onboarding;

/// <summary>
/// Reads and updates the onboarding profile on the Dynamics account.
/// Company name stays in SQL. These columns are matched by the logical names
/// on each onboarding field.
/// </summary>
public sealed class DataverseAccountProfile
{
    private readonly Dictionary<string, List<string>> _choiceCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _configuration;
    private ClientSecretCredential? _credential;

    public DataverseAccountProfile(IHttpClientFactory http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<CrmProfileRead> ReadAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var (mapped, unmapped) = Map();
        var values = Blank();
        var choices = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        if (mapped.Count == 0)
            return new CrmProfileRead(values, unmapped, choices);

        var select = string.Join(",", mapped.Select(SelectName).Distinct(StringComparer.OrdinalIgnoreCase));
        using var document = await SendAsync(
            HttpMethod.Get,
            $"accounts({accountId:D})?$select={select}",
            body: null,
            formatted: true,
            cancellationToken);

        var account = document.RootElement;
        foreach (var field in mapped)
        {
            values[field.Key] = ReadValue(account, field);
            if (field.AttributeType is "Picklist" or "MultiSelectPicklist")
            {
                try
                {
                    choices[field.Key] = await ChoiceLabelsAsync(field, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        return new CrmProfileRead(values, unmapped, choices);
    }

    public async Task<string?> WriteAsync(
        Guid accountId,
        IReadOnlyDictionary<string, string?> fields,
        CancellationToken cancellationToken)
    {
        var (mapped, unmapped) = Map();
        var current = await ReadAsync(accountId, cancellationToken);
        var patch = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var problems = new List<string>();

        foreach (var field in mapped)
        {
            fields.TryGetValue(field.Key, out var submitted);
            var next = submitted?.Trim() ?? "";
            var previous = current.Values.TryGetValue(field.Key, out var existing) ? existing?.Trim() ?? "" : "";
            if (string.Equals(previous, next, StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                await AddPatchAsync(patch, field, next, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                problems.Add(ex.Message);
            }
        }

        if (problems.Count > 0)
            throw new InvalidOperationException(string.Join(" ", problems));

        if (patch.Count > 0)
        {
            var json = JsonSerializer.Serialize(patch, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            });
            using var response = await SendAsync(
                HttpMethod.Patch,
                $"accounts({accountId:D})",
                json,
                formatted: false,
                cancellationToken);
        }

        var skipped = unmapped
            .Where(label => fields.Any(pair =>
                string.Equals(LabelFor(pair.Key), label, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(pair.Value)))
            .ToList();

        return skipped.Count == 0
            ? null
            : "Saved to Dynamics. No account column matched: " + string.Join(", ", skipped) + ".";
    }

    private async Task AddPatchAsync(Dictionary<string, object?> patch, MappedField field, string value, CancellationToken cancellationToken)
    {
        var type = field.AttributeType;
        if (string.IsNullOrEmpty(value))
        {
            if (type is "Lookup" or "Customer" or "Owner")
                patch[$"{field.LogicalName}@odata.bind"] = null;
            else if (type == "Boolean")
                patch[field.LogicalName] = null;
            else
                patch[field.LogicalName] = null;
            return;
        }

        switch (type)
        {
            case "String":
            case "Memo":
                patch[field.LogicalName] = value;
                return;
            case "Boolean":
                if (!IsYes(value) && !IsNo(value))
                    throw new InvalidOperationException($"{field.Label} must be Yes or No.");
                patch[field.LogicalName] = IsYes(value);
                return;
            case "Integer":
            case "BigInt":
                if (!long.TryParse(value, out var whole))
                    throw new InvalidOperationException($"{field.Label} must be a whole number.");
                patch[field.LogicalName] = whole;
                return;
            case "Decimal":
            case "Double":
            case "Money":
                if (!decimal.TryParse(value, out var number))
                    throw new InvalidOperationException($"{field.Label} must be a number.");
                patch[field.LogicalName] = number;
                return;
            case "DateTime":
                if (!DateTime.TryParse(value, out var date))
                    throw new InvalidOperationException($"{field.Label} must be a date.");
                patch[field.LogicalName] = date.ToString("yyyy-MM-dd");
                return;
            case "Picklist":
            case "State":
            case "Status":
                var option = await ResolveOptionAsync(field, value, cancellationToken);
                patch[field.LogicalName] = option;
                return;
            case "MultiSelectPicklist":
                var parts = value.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var codes = new List<int>();
                foreach (var part in parts)
                    codes.Add(await ResolveOptionAsync(field, part, cancellationToken));
                patch[field.LogicalName] = string.Join(",", codes);
                return;
            case "Lookup":
            case "Customer":
            case "Owner":
                var bind = await ResolveLookupAsync(field, value, cancellationToken);
                patch[$"{field.LogicalName}@odata.bind"] = bind;
                return;
            default:
                throw new InvalidOperationException($"{field.Label} cannot be written from the portal ({type}).");
        }
    }

    private static (IReadOnlyList<MappedField> Mapped, IReadOnlyList<string> Unmapped) Map()
    {
        var mapped = OnboardingCatalog.InputFields
            .Select(field => new MappedField(field.Key, field.Label, field.LogicalName, AttributeType(field.Kind)))
            .ToList();
        return (mapped, Array.Empty<string>());
    }

    private static string AttributeType(OnboardingKind kind) => kind switch
    {
        OnboardingKind.YesNo => "Boolean",
        OnboardingKind.Dropdown => "Picklist",
        OnboardingKind.MultiSelect => "MultiSelectPicklist",
        OnboardingKind.Date => "DateTime",
        OnboardingKind.Number => "Integer",
        OnboardingKind.Lookup => "Lookup",
        _ => "String"
    };

    private static string SelectName(MappedField field) =>
        field.AttributeType is "Lookup" or "Customer" or "Owner"
            ? $"_{field.LogicalName}_value"
            : field.LogicalName;

    private async Task<List<string>> ChoiceLabelsAsync(MappedField field, CancellationToken cancellationToken)
    {
        if (_choiceCache.TryGetValue(field.LogicalName, out var cached))
            return cached;

        var labels = new List<string>();
        using var document = await OptionSetAsync(field, cancellationToken);
        foreach (var setName in new[] { "OptionSet", "GlobalOptionSet" })
        {
            if (!document.RootElement.TryGetProperty(setName, out var set) || set.ValueKind != JsonValueKind.Object)
                continue;
            if (!set.TryGetProperty("Options", out var options) || options.ValueKind != JsonValueKind.Array)
                continue;
            foreach (var option in options.EnumerateArray())
            {
                var text = option.TryGetProperty("Label", out var optionLabel) ? LabelOf(optionLabel) : null;
                if (!string.IsNullOrWhiteSpace(text))
                    labels.Add(text.Trim());
            }

            if (labels.Count > 0)
                break;
        }

        _choiceCache[field.LogicalName] = labels;
        return labels;
    }

    private async Task<int> ResolveOptionAsync(MappedField field, string label, CancellationToken cancellationToken)
    {
        using var document = await OptionSetAsync(field, cancellationToken);

        foreach (var setName in new[] { "OptionSet", "GlobalOptionSet" })
        {
            if (!document.RootElement.TryGetProperty(setName, out var set) || set.ValueKind != JsonValueKind.Object)
                continue;
            if (!set.TryGetProperty("Options", out var options) || options.ValueKind != JsonValueKind.Array)
                continue;
            foreach (var option in options.EnumerateArray())
            {
                var text = option.TryGetProperty("Label", out var optionLabel) ? LabelOf(optionLabel) : null;
                if (!SameLabel(text, label))
                    continue;
                return option.GetProperty("Value").GetInt32();
            }
        }

        throw new InvalidOperationException($"{field.Label} does not have a Dynamics choice named \"{label}\".");
    }

    private async Task<string> ResolveLookupAsync(MappedField field, string name, CancellationToken cancellationToken)
    {
        using var meta = await SendAsync(
            HttpMethod.Get,
            $"EntityDefinitions(LogicalName='account')/Attributes(LogicalName='{field.LogicalName}')/Microsoft.Dynamics.CRM.LookupAttributeMetadata?$select=Targets",
            body: null,
            formatted: false,
            cancellationToken);

        var target = "systemuser";
        if (meta.RootElement.TryGetProperty("Targets", out var targets) && targets.ValueKind == JsonValueKind.Array)
        {
            var first = targets.EnumerateArray().Select(item => item.GetString()).FirstOrDefault(item => !string.IsNullOrWhiteSpace(item));
            if (!string.IsNullOrWhiteSpace(first))
                target = first;
        }

        using var entity = await SendAsync(
            HttpMethod.Get,
            $"EntityDefinitions(LogicalName='{target}')?$select=EntitySetName,PrimaryIdAttribute,PrimaryNameAttribute",
            body: null,
            formatted: false,
            cancellationToken);

        var setName = entity.RootElement.GetProperty("EntitySetName").GetString();
        var idAttribute = entity.RootElement.GetProperty("PrimaryIdAttribute").GetString();
        var nameAttribute = entity.RootElement.GetProperty("PrimaryNameAttribute").GetString();
        var safe = name.Replace("'", "''");
        var filter = Uri.EscapeDataString($"{nameAttribute} eq '{safe}'");
        using var rows = await SendAsync(
            HttpMethod.Get,
            $"{setName}?$select={idAttribute}&$filter={filter}&$top=2",
            body: null,
            formatted: false,
            cancellationToken);

        var matches = rows.RootElement.GetProperty("value").EnumerateArray().ToList();
        if (matches.Count != 1 || string.IsNullOrWhiteSpace(idAttribute) || string.IsNullOrWhiteSpace(setName))
            throw new InvalidOperationException($"Dynamics has no single {field.Label} named \"{name}\".");

        var id = matches[0].GetProperty(idAttribute).GetGuid();
        return $"/{setName}({id:D})";
    }

    private static Dictionary<string, string?> Blank()
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in OnboardingCatalog.InputFields)
            values[field.Key] = "";
        return values;
    }

    private Task<JsonDocument> OptionSetAsync(MappedField field, CancellationToken cancellationToken)
    {
        var cast = field.AttributeType switch
        {
            "State" => "Microsoft.Dynamics.CRM.StateAttributeMetadata",
            "Status" => "Microsoft.Dynamics.CRM.StatusAttributeMetadata",
            "MultiSelectPicklist" => "Microsoft.Dynamics.CRM.MultiSelectPicklistAttributeMetadata",
            _ => "Microsoft.Dynamics.CRM.PicklistAttributeMetadata"
        };
        return SendAsync(
            HttpMethod.Get,
            $"EntityDefinitions(LogicalName='account')/Attributes(LogicalName='{field.LogicalName}')/{cast}?$select=LogicalName&$expand=OptionSet($select=Options),GlobalOptionSet($select=Options)",
            body: null,
            formatted: false,
            cancellationToken);
    }

    private static string? ReadValue(JsonElement account, MappedField field)
    {
        var property = SelectName(field);
        if (!account.TryGetProperty(property, out var raw) || raw.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return "";

        if (field.AttributeType is "Picklist" or "State" or "Status" or "MultiSelectPicklist" or "Lookup" or "Customer" or "Owner")
        {
            var formattedName = property + "@OData.Community.Display.V1.FormattedValue";
            if (account.TryGetProperty(formattedName, out var formatted) && formatted.ValueKind == JsonValueKind.String)
                return formatted.GetString()?.Trim() ?? "";
            return "";
        }

        return field.AttributeType switch
        {
            "Boolean" => raw.ValueKind == JsonValueKind.True ? "Yes" : "No",
            "DateTime" when raw.ValueKind == JsonValueKind.String && DateTime.TryParse(raw.GetString(), out var date) => date.ToString("yyyy-MM-dd"),
            "Integer" or "BigInt" or "Decimal" or "Double" or "Money" => raw.ToString(),
            _ => raw.ValueKind == JsonValueKind.String ? raw.GetString()?.Trim() ?? "" : raw.ToString()
        };
    }

    private async Task<JsonDocument> SendAsync(
        HttpMethod method,
        string path,
        string? body,
        bool formatted,
        CancellationToken cancellationToken)
    {
        var org = OrganizationUrl();
        var request = new HttpRequestMessage(method, org + "/api/data/v9.2/" + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await TokenAsync(org, cancellationToken));
        request.Headers.Add("OData-MaxVersion", "4.0");
        request.Headers.Add("OData-Version", "4.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (formatted)
            request.Headers.TryAddWithoutValidation("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
        if (method == HttpMethod.Patch)
            request.Headers.TryAddWithoutValidation("If-Match", "*");
        if (body is not null)
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var client = _http.CreateClient("Dataverse");
        using var response = await client.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(ErrorMessage(response.StatusCode, payload));

        if (string.IsNullOrWhiteSpace(payload))
            return JsonDocument.Parse("{}");

        return JsonDocument.Parse(payload);
    }

    private async Task<string> TokenAsync(string org, CancellationToken cancellationToken)
    {
        var tenant = First(_configuration["Dataverse:TenantId"], _configuration["AzureAd:TenantId"])?.Trim();
        var clientId = _configuration["Dataverse:ClientId"]?.Trim();
        var secret = _configuration["Dataverse:ClientSecret"]?.Trim();
        if (!IsSet(tenant) || !IsSet(clientId) || !IsSet(secret))
        {
            throw new InvalidOperationException(
                "Dynamics is not configured. On the API, set Dataverse__EnvironmentUrl, Dataverse__ClientId, and Dataverse__ClientSecret.");
        }

        _credential ??= new ClientSecretCredential(tenant, clientId, secret);
        try
        {
            var token = await _credential.GetTokenAsync(new TokenRequestContext([$"{org}/.default"]), cancellationToken);
            return token.Token;
        }
        catch (Exception ex)
        {
            _credential = null;
            throw new InvalidOperationException("Dynamics sign-in failed. " + Detail(ex), ex);
        }
    }

    private static string Detail(Exception ex)
    {
        var parts = new List<string>();
        for (var current = ex; current is not null && parts.Count < 4; current = current.InnerException)
        {
            var message = current.Message?.Trim();
            if (!string.IsNullOrWhiteSpace(message) && !parts.Contains(message))
                parts.Add(message);
        }

        return parts.Count == 0 ? ex.GetType().Name : string.Join(" ", parts);
    }

    private string OrganizationUrl()
    {
        var url = _configuration["Dataverse:EnvironmentUrl"]?.Trim().TrimEnd('/');
        if (!IsSet(url) || !url!.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Dynamics is not configured. On the API, set Dataverse__EnvironmentUrl to the environment URL, such as https://yourorg.crm.dynamics.com.");
        }

        return url;
    }

    private static string ErrorMessage(System.Net.HttpStatusCode status, string payload)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (document.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message) &&
                message.ValueKind == JsonValueKind.String)
            {
                return $"Dynamics ({(int)status}): {message.GetString()}";
            }
        }
        catch (JsonException)
        {
        }

        var detail = string.IsNullOrWhiteSpace(payload) ? status.ToString() : payload;
        return $"Dynamics ({(int)status}): {detail}";
    }

    private static string? LabelOf(JsonElement label)
    {
        if (label.ValueKind != JsonValueKind.Object)
            return null;
        if (label.TryGetProperty("UserLocalizedLabel", out var user) &&
            user.ValueKind == JsonValueKind.Object &&
            user.TryGetProperty("Label", out var userText) &&
            userText.ValueKind == JsonValueKind.String)
        {
            return userText.GetString();
        }

        if (!label.TryGetProperty("LocalizedLabels", out var labels) || labels.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var item in labels.EnumerateArray())
        {
            if (item.TryGetProperty("Label", out var text) && text.ValueKind == JsonValueKind.String)
                return text.GetString();
        }

        return null;
    }

    private static string LabelFor(string key) =>
        OnboardingCatalog.InputFields.FirstOrDefault(field => string.Equals(field.Key, key, StringComparison.OrdinalIgnoreCase))?.Label
        ?? key;

    private static bool IsYes(string value) =>
        value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
        value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
        value == "1";

    private static bool IsNo(string value) =>
        value.Equals("no", StringComparison.OrdinalIgnoreCase) ||
        value.Equals("false", StringComparison.OrdinalIgnoreCase) ||
        value == "0";

    private static bool SameLabel(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.Ordinal);

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";
        var chars = value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray();
        return new string(chars);
    }

    private static string? First(string? primary, string? fallback) =>
        IsSet(primary) ? primary : fallback;

    private static bool IsSet(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        !value.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase);

    private sealed record MappedField(string Key, string Label, string LogicalName, string AttributeType);
}

public sealed record CrmProfileRead(
    Dictionary<string, string?> Values,
    IReadOnlyList<string> UnmappedLabels,
    Dictionary<string, List<string>> Choices);
