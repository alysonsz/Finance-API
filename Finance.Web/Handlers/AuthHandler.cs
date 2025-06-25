using System.Net.Http.Json;
using Microsoft.JSInterop;
using Finance.Contracts.Requests.Auth;

namespace Finance.Web.Handlers;

public class AuthHandler
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public AuthHandler(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (content is null || string.IsNullOrWhiteSpace(content.Token))
            return false;

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, content.Token);
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

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, content.Token);
        return true;
    }

    public async Task LogoutAsync()
        => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

    public async Task<string?> GetTokenAsync()
        => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
