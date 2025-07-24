using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Commands.Users;

public class GetProfileCommand : IRequest<Response<UserProfileResponse?>>
{
}
