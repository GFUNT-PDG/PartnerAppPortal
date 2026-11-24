using System.Net;
using System.Text;

namespace PartnerAppPortal.Api.Crm;

public sealed class CrmSyncStarter
{
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _configuration;

    public CrmSyncStarter(IHttpClientFactory http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<string> StartAsync(CancellationToken cancellationToken)
    {
        var url = _configuration["CrmSync:TriggerUrl"]?.Trim();
        if (string.IsNullOrWhiteSpace(url) ||
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            url.Contains("YOUR_", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "CRM sync is not configured. On the API, set CrmSync__TriggerUrl to the flow POST URL.");
        }

        var client = _http.CreateClient("CrmSync");
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException(
                "The sync flow did not answer in time. On the HTTP trigger, turn on Asynchronous response, then try again.");
        }

        using (response)
        {
            if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.NoContent)
            {
                return "Sync started. New and updated accounts appear in the customer list after the flow finishes.";
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (body.Contains("sig=", StringComparison.OrdinalIgnoreCase))
                body = "";
            if (body.Length > 300)
                body = body[..300];

            throw new InvalidOperationException(
                $"The sync flow returned {(int)response.StatusCode}. {body}".Trim());
        }
    }
}
