using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using PartnerAppPortal.Portal.Services;

namespace PartnerAppPortal.Portal.Controllers;

[Authorize]
[Route("partner/site-reports/file")]
public sealed class SiteReportController : Controller
{
    private readonly SiteReportAccess _access;
    private readonly TeamsGraphClient _teams;

    public SiteReportController(SiteReportAccess access, TeamsGraphClient teams)
    {
        _access = access;
        _teams = teams;
    }

    [HttpGet("{customerId:guid}/{*path}")]
    public async Task<IActionResult> Get(
        Guid customerId,
        string? path,
        [FromQuery] bool download = false)
    {
        try
        {
            var customer = await _access.ResolveAsync(User, customerId);
            if (customer is null || string.IsNullOrWhiteSpace(customer.Name))
                return NotFound("Partner account was not found.");

            if (string.IsNullOrWhiteSpace(path))
                return BadRequest();

            var file = await _teams.DownloadAccountFileByPathAsync(customer.Name, path, User);
            if (file is null)
                return NotFound();

            Response.Headers.ContentSecurityPolicy = "frame-ancestors 'self'";
            Response.Headers.XFrameOptions = "SAMEORIGIN";
            Response.Headers.CacheControl = "private, no-store";

            var contentType = SiteReportFiles.ContentTypeFor(file.FileName, file.ContentType);
            if (download)
                return File(file.Bytes, contentType, file.FileName);

            return File(file.Bytes, contentType);
        }
        catch (MicrosoftIdentityWebChallengeUserException)
        {
            var returnUrl = $"{Request.Path}{Request.QueryString}";
            return Redirect($"/partner/teams/connect?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }
}
