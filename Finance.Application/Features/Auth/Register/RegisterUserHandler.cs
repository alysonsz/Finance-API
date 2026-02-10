using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Auth.Register;

public sealed class RegisterUserHandler(IUserRepository userRepository) : IRequestHandler<RegisterUserCommand, Response<string>>
{
    public async Task<Response<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Response<string>.Fail("O email informado já está em uso.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await userRepository.AddAsync(user);

        return Response<string>.Success("Usuário registrado com sucesso. Utilize suas credenciais para efetuar o login.");
    }
}
