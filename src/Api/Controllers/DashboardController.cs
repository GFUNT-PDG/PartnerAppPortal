using Microsoft.AspNetCore.Mvc;

namespace PartnerAppPortal.Api.Controllers.Modules;

[ApiController]
[Route("api/modules/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Dashboard module is online");
}
