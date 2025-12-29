using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;

namespace Finance.Web.Interfaces;

public interface IAuthHandler
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<Response<UserProfileResponse?>> GetProfileAsync();
    Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request);
    Task<string?> GetTokenAsync();
    Task<bool> TryRefreshTokenAsync();
}
