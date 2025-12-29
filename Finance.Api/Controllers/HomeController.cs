using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Home()
    {
        return Ok(new { message = "ok" });
    }
}
