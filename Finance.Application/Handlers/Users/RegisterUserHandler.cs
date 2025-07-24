using Finance.Application.Commands.Users;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Handlers.Users;

public class RegisterUserHandler(IUserRepository userRepository, ITokenService tokenService)
    : IRequestHandler<RegisterUserCommand, Response<LoginResponse?>>
{
    public async Task<Response<LoginResponse?>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Response<LoginResponse?>.Fail("O email informado já está em uso.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await userRepository.AddAsync(user);

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

        return Response<LoginResponse?>.Success(loginResponse, "Usuário registrado com sucesso!");
    }
}
