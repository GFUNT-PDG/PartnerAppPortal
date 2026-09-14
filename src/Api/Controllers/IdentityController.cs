using Microsoft.AspNetCore.Mvc;

namespace PartnerAppPortal.Api.Controllers.Modules;

[ApiController]
[Route("api/modules/identity")]
public class IdentityController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Identity module is online");
}
