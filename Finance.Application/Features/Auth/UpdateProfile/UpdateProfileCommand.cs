using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Features.Auth.UpdateProfile;

public class UpdateProfileCommand : IRequest<Response<UserProfileResponse?>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
