using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Commands.Users;

public class UpdateProfileCommand : IRequest<Response<UserProfileResponse?>>
{
    public string Name { get; set; } = string.Empty;
}
