using System.Net.Http.Headers;
using Microsoft.Identity.Web;

namespace PartnerAppPortal.Portal.Services;

public sealed class ApiTokenHandler : DelegatingHandler
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ApiTokenHandler(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // App-only token. Partner users stay signed in with their own tenant.
        // Their tenant is never asked to consent to PAP-Api.
        var scope = _configuration["DownstreamApi:AppScope"];
        var tenant = _configuration["Graph:ResourceTenantId"];
        if (IsConfigured(scope) && IsConfigured(tenant))
        {
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync(
                scope!.Trim(),
                tenant: tenant!.Trim());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            throw new InvalidOperationException(
                "Portal is not sending an API token. On the Portal app, set DownstreamApi__AppScope " +
                "and Graph__ResourceTenantId to the PDG tenant id.");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsConfigured(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        !value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);

    internal static bool IsConsentChallenge(Exception ex)
    {
        for (Exception? current = ex; current != null; current = current.InnerException)
        {
            if (current is MicrosoftIdentityWebChallengeUserException)
                return true;
        }

        return false;
    }
}
