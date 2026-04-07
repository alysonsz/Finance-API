using Finance.Application.Extensions;
using Finance.Application.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Features.Auth.UpdateProfile;

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

        var updateResult = user.UpdateProfile(request.Name, user.Email);
        if (updateResult.IsFailure)
            return Response<UserProfileResponse?>.Fail(string.Join("; ", updateResult.Errors));

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
