using Finance.Application.Features.Auth.GetProfile;
using Finance.Application.Features.Auth.Login;
using Finance.Application.Features.Auth.RefreshToken;
using Finance.Application.Features.Auth.Register;
using Finance.Application.Features.Auth.UpdateProfile;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using MediatR;

namespace Finance.Application.Services;

public sealed class UserService(IMediator mediator) : IUserService
{
    private readonly IMediator _mediator = mediator;

    public Task<Response<LoginResponse?>> LoginAsync(LoginRequest request)
        => _mediator.Send(new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        });

    public Task<Response<string>> RegisterAsync(RegisterRequest request)
        => _mediator.Send(new RegisterUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        });

    public Task<Response<UserProfileResponse?>> GetProfileAsync()
        => _mediator.Send(new GetProfileCommand());

    public Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request)
        => _mediator.Send(new UpdateProfileCommand
        {
            Name = request.Name,
            Email = request.Email
        });

    public Task<Response<LoginResponse?>> RefreshTokenAsync(string accessToken, string refreshToken)
        => _mediator.Send(new RefreshUserTokenCommand
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
}
