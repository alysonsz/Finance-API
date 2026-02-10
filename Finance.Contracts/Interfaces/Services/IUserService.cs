using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;

namespace Finance.Contracts.Interfaces.Services;

public interface IUserService
{
    Task<Response<LoginResponse?>> LoginAsync(LoginRequest request);
    Task<Response<string>> RegisterAsync(RegisterRequest request);
    Task<Response<UserProfileResponse?>> GetProfileAsync();
    Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request);
    Task<Response<LoginResponse?>> RefreshTokenAsync(string accessToken, string refreshToken);
}

