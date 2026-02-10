using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Features.Auth.RefreshToken;

public class RefreshUserTokenHandler(IUserRepository userRepository, ITokenService tokenService)
    : IRequestHandler<RefreshUserTokenCommand, Response<LoginResponse?>>
{
    public async Task<Response<LoginResponse?>> Handle(RefreshUserTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

            if (principal?.Identity?.Name == null)
            {
                return Response<LoginResponse?>.Fail("Token inválido ou expirado.");
            }

            var userId = long.Parse(principal.Identity.Name);

            var user = await userRepository.GetByIdAsync(userId);

            if (user is null ||
                user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Response<LoginResponse?>.Fail("Token inválido ou expirado.");
            }

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userRepository.UpdateAsync(user);

            var loginResponse = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return Response<LoginResponse?>.Success(loginResponse, "Token renovado com sucesso.");
        }
        catch (Exception)
        {
            return Response<LoginResponse?>.Fail("Token inválido ou expirado.");
        }
    }
}
