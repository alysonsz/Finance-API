using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Features.Auth.GetProfile;

public class GetProfileCommand : IRequest<Response<UserProfileResponse?>>
{
}
