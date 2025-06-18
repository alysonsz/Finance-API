using Microsoft.AspNetCore.Mvc;
using Finance.Application.Interfaces.Handlers;
using Finance.Application.Requests.Auth;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserHandler _userHandler;

    public AuthController(IUserHandler userHandler)
    {
        _userHandler = userHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userHandler.LoginAsync(request);

        if (!response.IsSuccess)
            return Unauthorized(response.Message);

        return Ok(new { token = response.Data });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _userHandler.RegisterAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response.Message);

        return Ok(new { token = response.Data });
    }
}
