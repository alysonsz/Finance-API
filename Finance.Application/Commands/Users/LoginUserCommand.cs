using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Commands.Users;

public class LoginUserCommand : IRequest<Response<LoginResponse?>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
