using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Features.Auth.Login;

public class LoginUserCommand : IRequest<Response<LoginResponse?>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
