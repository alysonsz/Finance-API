using Finance.API.Extensions;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.LoginAsync(request);

        if (!response.IsSuccess)
            return Unauthorized(response.Message);

        return Ok(response.Data);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _userService.RegisterAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response.Message);

        return Ok(new { message = response.Data });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var response = await _userService.GetProfileAsync();
        return this.FromResponse(response);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserProfileRequest request)
    {
        var response = await _userService.UpdateProfileAsync(request);
        return this.FromResponse(response);
    }
}
