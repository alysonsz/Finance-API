using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;

namespace Finance.Contracts.Interfaces.Handlers;

public interface IUserHandler
{
    Task<Response<string>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
    Task<Response<UserProfileResponse?>> GetProfileAsync();
    Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request);
    Task<Response<LoginResponse?>> RefreshTokenAsync(string accessToken, string refreshToken);
}
