using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Auth.Register;

public class RegisterUserCommand : IRequest<Response<string>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
