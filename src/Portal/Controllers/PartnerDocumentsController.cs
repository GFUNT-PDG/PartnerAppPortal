using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using PartnerAppPortal.Portal.Services;

namespace PartnerAppPortal.Portal.Controllers;

[Authorize]
[Route("partner/documents/file")]
public sealed class PartnerDocumentsController : Controller
{
    private readonly SiteReportAccess _access;
    private readonly TeamsGraphClient _teams;

    public PartnerDocumentsController(SiteReportAccess access, TeamsGraphClient teams)
    {
        _access = access;
        _teams = teams;
    }

    [HttpGet("{customerId:guid}")]
    public async Task<IActionResult> Download(
        Guid customerId,
        [FromQuery] string driveId,
        [FromQuery] string itemId)
    {
        try
        {
            var customer = await _access.ResolveAsync(User, customerId);
            if (customer is null || string.IsNullOrWhiteSpace(customer.Name))
                return Content("Partner account was not found.", "text/plain");

            if (string.IsNullOrWhiteSpace(driveId) || string.IsNullOrWhiteSpace(itemId))
                return Content("Missing driveId or itemId.", "text/plain");

            var downloadUrl = await _teams.GetAccountFileDownloadUrlAsync(
                customer.Name,
                driveId,
                itemId,
                User);

            return Redirect(downloadUrl);
        }
        catch (MicrosoftIdentityWebChallengeUserException)
        {
            var returnUrl = $"{Request.Path}{Request.QueryString}";
            return Redirect($"/partner/teams/connect?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
        catch (Exception ex)
        {
            // Plain 200 text so Chrome shows the real Graph/auth error instead of a blank 500 page.
            var message = ex.InnerException?.Message ?? ex.Message;
            return Content("Download failed: " + message, "text/plain");
        }
    }
}
