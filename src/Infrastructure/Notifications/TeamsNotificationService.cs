using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.SalesAccelerator;
using PartnerAppPortal.Core.Options;

namespace PartnerAppPortal.Infrastructure.Notifications;

public class TeamsNotificationService : ITeamsNotificationService
{
    private readonly HttpClient _http;
    private readonly TeamsNotificationOptions _options;
    private readonly ILogger<TeamsNotificationService> _logger;

    public TeamsNotificationService(
        HttpClient http,
        IOptions<TeamsNotificationOptions> options,
        ILogger<TeamsNotificationService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public Task NotifyNewSubmissionAsync(
        string partner,
        string submittedBy,
        SalesAcceleratorSubmitResult result,
        CancellationToken cancellationToken = default)
    {
        var text = new StringBuilder()
            .AppendLine("Sales Accelerator Accounts — new template submitted")
            .AppendLine($"Partner: {partner}")
            .AppendLine($"Submitted by: {submittedBy}")
            .Append($"Inserted: {result.InsertedCount} · Duplicates skipped: {result.DuplicateCount} · Invalid rows: {result.InvalidCount}")
            .ToString();

        return PostAsync(_options.PartnerDevelopmentTeamWebhook, "New Sales Accelerator submission", text, cancellationToken);
    }

    public Task NotifyDuplicateDomainsAsync(
        string submittingPartner,
        string submittedBy,
        IReadOnlyList<SalesAcceleratorDuplicate> duplicates,
        CancellationToken cancellationToken = default)
    {
        if (duplicates.Count == 0)
            return Task.CompletedTask;

        var text = new StringBuilder()
            .AppendLine("Sales Accelerator Accounts — duplicate domain(s) skipped")
            .AppendLine($"Submitting partner: {submittingPartner}")
            .AppendLine($"Submitted by: {submittedBy}");

        foreach (var d in duplicates)
            text.AppendLine($"• Domain {d.Domain} already associated to partner {d.CurrentPartner}");

        return PostAsync(
            _options.PartnerDevelopmentChannelWebhook,
            "Sales Accelerator duplicate domain",
            text.ToString().TrimEnd(),
            cancellationToken);
    }

    private async Task PostAsync(string? webhook, string summary, string bodyText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(webhook))
        {
            _logger.LogWarning("Teams webhook is not configured. Skipping notification: {Summary}", summary);
            return;
        }

        try
        {
            var payload = AdaptiveCardPayload(summary, bodyText);
            using var response = await _http.PostAsJsonAsync(webhook.Trim(), payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Teams webhook failed ({Status}): {Body}",
                    (int)response.StatusCode,
                    body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Teams webhook call failed for {Summary}", summary);
        }
    }

    private static object AdaptiveCardPayload(string title, string bodyText)
    {
        return new
        {
            type = "message",
            attachments = new object[]
            {
                new
                {
                    contentType = "application/vnd.microsoft.card.adaptive",
                    contentUrl = (string?)null,
                    content = new Dictionary<string, object?>
                    {
                        ["$schema"] = "http://adaptivecards.io/schemas/adaptive-card.json",
                        ["type"] = "AdaptiveCard",
                        ["version"] = "1.4",
                        ["body"] = new object[]
                        {
                            new { type = "TextBlock", text = title, weight = "Bolder", wrap = true, size = "Medium" },
                            new { type = "TextBlock", text = bodyText, wrap = true }
                        }
                    }
                }
            }
        };
    }
}
