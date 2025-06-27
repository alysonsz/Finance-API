using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Finance.Web.Handlers;

public class AuthHandler(HttpClient httpClient, IJSRuntime jsRuntime)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private const string _tokenKey = "authToken";

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (content is null || string.IsNullOrWhiteSpace(content.Token))
            return false;

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _tokenKey, content.Token);
        return true;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/auth/register", request);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (content is null || string.IsNullOrWhiteSpace(content.Token))
            return false;

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _tokenKey, content.Token);
        return true;
    }

    public async Task<Response<UserProfileResponse?>> GetProfileAsync()
    {
        var response = await _httpClient.GetAsync("v1/auth/profile");
        return await response.Content.ReadFromJsonAsync<Response<UserProfileResponse?>>() ?? Response<UserProfileResponse?>.Fail("Erro");
    }

    public async Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("v1/auth/profile", request);
        return await response.Content.ReadFromJsonAsync<Response<UserProfileResponse?>>() ?? Response<UserProfileResponse?>.Fail("Erro");
    }

    public async Task LogoutAsync()
        => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _tokenKey);

    public async Task<string?> GetTokenAsync()
        => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _tokenKey);
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
