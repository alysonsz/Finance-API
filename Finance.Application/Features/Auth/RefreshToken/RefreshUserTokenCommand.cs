using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Features.Auth.RefreshToken;

public class RefreshUserTokenCommand : IRequest<Response<LoginResponse?>>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
