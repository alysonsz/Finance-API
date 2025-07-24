using Finance.Application.Commands.Users;
using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Handlers.Users;

public class UpdateProfileHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateProfileCommand, Response<UserProfileResponse?>>
{
    public async Task<Response<UserProfileResponse?>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            return Response<UserProfileResponse?>.Fail("HttpContext is not available.");
        }

        var userId = httpContext.User.GetUserId();
        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return Response<UserProfileResponse?>.Fail("Usuário não encontrado.");
        }

        user.Name = request.Name;

        var updatedUser = await userRepository.UpdateAsync(user);

        var dto = new UserProfileResponse
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Email = updatedUser.Email
        };

        return Response<UserProfileResponse?>.Success(dto, "Perfil atualizado com sucesso!");
    }
}
