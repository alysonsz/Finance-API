using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Features.Auth.GetProfile;

public class GetProfileHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetProfileCommand, Response<UserProfileResponse?>>
{
    public async Task<Response<UserProfileResponse?>> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User.GetUserId();

        if (userId == null)
        {
            return Response<UserProfileResponse?>.Fail("User ID not found.");
        }

        var user = await userRepository.GetByIdAsync((long)userId);

        if (user == null)
        {
            return Response<UserProfileResponse?>.Fail("Usuário não encontrado.");
        }

        var dto = new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        return Response<UserProfileResponse?>.Success(dto);
    }
}
