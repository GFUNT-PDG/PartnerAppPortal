using Microsoft.AspNetCore.Mvc;

namespace PartnerAppPortal.Api.Controllers
{
    [ApiController]
    [Route("api/ping")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("pong");
    }
}
