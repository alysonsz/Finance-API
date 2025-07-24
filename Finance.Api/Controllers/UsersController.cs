using Finance.API.Extensions;
using Finance.Application.Commands.Users;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public class UsersController(IMediator mediator, IUserHandler userHandler) : ControllerBase
{
    private readonly IUserHandler _userHandler = userHandler;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshUserTokenCommand command)
    {
        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var query = new GetProfileCommand();
        var response = await mediator.Send(query);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
    {
        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
