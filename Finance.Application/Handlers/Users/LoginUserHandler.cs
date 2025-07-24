using Finance.Application.Commands.Users;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Handlers.Users;

public class LoginUserHandler(IUserRepository userRepository, ITokenService tokenService)  
    : IRequestHandler<LoginUserCommand, Response<LoginResponse?>>
{
    public async Task<Response<LoginResponse?>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Response<LoginResponse?>.Fail("Credenciais inválidas.");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await userRepository.UpdateAsync(user);

        var loginResponse = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Response<LoginResponse?>.Success(loginResponse, "Login bem-sucedido!");
    }
}
