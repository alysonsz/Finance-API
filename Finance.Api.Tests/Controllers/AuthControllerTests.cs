using Finance.Contracts.Requests.Auth;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Finance.Api.Tests.Controllers;

[Collection("Shared Test Collection")]
public class AuthControllerTests(CustomWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_Should_ReturnOkAndToken_When_RequestIsValid()
    {
        var request = new RegisterRequest
        {
            Name = "Test User Registration",
            Email = $"test-{Guid.NewGuid()}@email.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseBody.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_CredentialsAreInvalid()
    {
        var request = new LoginRequest
        {
            Email = "invalid@user.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_ReturnOkAndToken_When_CredentialsAreValid()
    {
        var password = "StrongPassword123!";
        var registerRequest = new RegisterRequest
        {
            Name = "Test User For Login",
            Email = $"login-test-{Guid.NewGuid()}@email.com",
            Password = password
        };
        // 1. Registra o usuário
        await _client.PostAsJsonAsync("v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("v1/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseBody.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetProfile_Should_ReturnOk_When_UserIsAuthenticated()
    {
        var response = await _client.GetAsync("v1/auth/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseBody.GetProperty("data").GetProperty("email").GetString().Should().Be("test@user.com");
    }
}