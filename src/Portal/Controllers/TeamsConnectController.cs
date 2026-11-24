using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace PartnerAppPortal.Portal.Controllers;

[Authorize]
[Route("partner/teams")]
public sealed class TeamsConnectController : Controller
{
    private static readonly string[] GraphScopes =
    {
        "https://graph.microsoft.com/Team.ReadBasic.All",
        "https://graph.microsoft.com/Channel.ReadBasic.All",
        "https://graph.microsoft.com/ChannelMessage.Read.All",
        "https://graph.microsoft.com/ChannelMessage.Send",
        "https://graph.microsoft.com/Files.Read.All"
    };

    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public TeamsConnectController(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    [HttpGet("connect")]
    [AuthorizeForScopes(Scopes = new[]
    {
        "https://graph.microsoft.com/Team.ReadBasic.All",
        "https://graph.microsoft.com/Channel.ReadBasic.All",
        "https://graph.microsoft.com/ChannelMessage.Read.All",
        "https://graph.microsoft.com/ChannelMessage.Send",
        "https://graph.microsoft.com/Files.Read.All"
    })]
    public async Task<IActionResult> Connect([FromQuery] string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "/dashboard/partner";

        var tenantId = _configuration["Graph:ResourceTenantId"];
        if (string.IsNullOrWhiteSpace(tenantId) || tenantId.Contains("YOUR_", StringComparison.Ordinal))
            tenantId = null;

        await _tokenAcquisition.GetAccessTokenForUserAsync(GraphScopes, tenantId: tenantId);
        return LocalRedirect(returnUrl);
    }
}
