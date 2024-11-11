using Microsoft.AspNetCore.Mvc;

namespace Beam.Api.Controllers;

[Route("api/ping")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok();
    }
}
