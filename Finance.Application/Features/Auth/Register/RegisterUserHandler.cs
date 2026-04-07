using Finance.Application.Interfaces.Repositories;
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

        var result = User.Create(request.Name, request.Email, BCrypt.Net.BCrypt.HashPassword(request.Password));
        if (result.IsFailure)
            return Response<string>.Fail(string.Join("; ", result.Errors));

        var user = result.Value;

        await userRepository.AddAsync(user);

        return Response<string>.Success("Usuário registrado com sucesso. Utilize suas credenciais para efetuar o login.");
    }
}
