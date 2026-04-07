using Finance.API.Extensions;
using Finance.Application.Features.Auth.GetProfile;
using Finance.Application.Features.Auth.Login;
using Finance.Application.Features.Auth.Register;
using Finance.Application.Features.Auth.UpdateProfile;
using Finance.Contracts.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var response = await mediator.Send(command);

        if (!response.IsSuccess)
            return Unauthorized(response.Message);

        return Ok(response.Data);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        var response = await mediator.Send(command);

        if (!response.IsSuccess)
            return BadRequest(response.Message);

        return Ok(new { message = response.Data });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var command = new GetProfileCommand();
        var response = await mediator.Send(command);
        return this.FromResponse(response);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserProfileRequest request)
    {
        var command = new UpdateProfileCommand
        {
            Name = request.Name
        };

        var response = await mediator.Send(command);
        return this.FromResponse(response);
    }
}
