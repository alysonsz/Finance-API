using Finance.API.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(IUserHandler userHandler) : ControllerBase
{
    private readonly IUserHandler _userHandler = userHandler;

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

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var response = await _userHandler.GetProfileAsync();
        return this.FromResponse(response);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfileAsync(UpdateUserProfileRequest request)
    {
        var response = await _userHandler.UpdateProfileAsync(request);
        return this.FromResponse(response);
    }
}
