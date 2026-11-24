using System.Text.Json;

namespace PartnerAppPortal.Portal.Services;

public sealed record IntroResultSection(
    string Name,
    int PointsEarned,
    int MaxPoints,
    decimal Percent);

public sealed record IntroResultRow(
    Guid Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    string Status,
    string? CompanyName,
    string PartnerId,
    string Email,
    string EmailDomain,
    string? SignedInEmail,
    string? TenantId,
    string? ReferrerName,
    string? CspContactEmail,
    bool IsPartial,
    string? SnapshotSource,
    decimal? OverallPercent,
    DateTimeOffset? NotificationSentAt,
    IReadOnlyList<IntroResultSection> Sections,
    IReadOnlyDictionary<string, string?> Answers);

public sealed class IntroScoreResultsClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public IntroScoreResultsClient(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_configuration["IntroScore:BaseUrl"]) &&
        !string.IsNullOrWhiteSpace(_configuration["IntroScore:ApiKey"]);

    public async Task<IReadOnlyList<IntroResultRow>> ListAsync(CancellationToken cancellationToken = default)
    {
        var key = _configuration["IntroScore:ApiKey"];
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/results");
        request.Headers.TryAddWithoutValidation("X-IntroScore-Key", key);
        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Intro Score report returned {(int)response.StatusCode}. {Trim(body)}");

        return JsonSerializer.Deserialize<List<IntroResultRow>>(body, JsonOptions) ?? [];
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string Trim(string body) =>
        body.Length <= 240 ? body : body[..240];
}
