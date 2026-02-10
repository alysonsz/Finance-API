using Finance.Application.Features.Auth.Login;
using Finance.Application.Features.Auth.Register;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Finance.Api.Tests.Controllers;

[Collection("Shared Test Collection")]
public class AuthControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_Should_ReturnOkAndToken_When_RequestIsValid()
    {
        var request = new RegisterUserCommand
        {
            Name = "Test User Registration",
            Email = $"test-{Guid.NewGuid()}@email.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("v1/auth/register", request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Falha no Register: {error}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.TryGetProperty("message", out var msg).Should().BeTrue();
        msg.GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_CredentialsAreInvalid()
    {
        var request = new LoginUserCommand
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
        var email = $"login-test-{Guid.NewGuid()}@email.com";

        await _client.PostAsJsonAsync("v1/auth/register", new RegisterUserCommand
        {
            Name = "Test User For Login",
            Email = email,
            Password = password
        });

        var response = await _client.PostAsJsonAsync("v1/auth/login", new LoginUserCommand
        {
            Email = email,
            Password = password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.TryGetProperty("accessToken", out var token).Should().BeTrue();

        token.ValueKind.Should().Be(JsonValueKind.String);
        token.GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetProfile_Should_ReturnOk_When_UserIsAuthenticated()
    {
        var email = $"profile-{Guid.NewGuid()}@email.com";
        var password = "Password123!";

        await _client.PostAsJsonAsync("v1/auth/register", new RegisterUserCommand
        {
            Name = "Profile User",
            Email = email,
            Password = password
        });

        var loginResponse = await _client.PostAsJsonAsync("v1/auth/login", new LoginUserCommand
        {
            Email = email,
            Password = password
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = ExtractAccessToken(loginResult);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("v1/auth/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static string ExtractAccessToken(JsonElement root)
    {
        if (root.TryGetProperty("accessToken", out var at) && at.ValueKind == JsonValueKind.String)
            return at.GetString() ?? throw new InvalidOperationException("accessToken veio null.");

        if (root.TryGetProperty("token", out var tokenEl))
        {
            if (tokenEl.ValueKind == JsonValueKind.String)
                return tokenEl.GetString() ?? throw new InvalidOperationException("Token string veio null.");

            if (tokenEl.ValueKind == JsonValueKind.Object &&
                tokenEl.TryGetProperty("accessToken", out var accessTokenEl) &&
                accessTokenEl.ValueKind == JsonValueKind.String)
            {
                return accessTokenEl.GetString() ?? throw new InvalidOperationException("accessToken veio null (token obj).");
            }
        }

        throw new InvalidOperationException($"Não consegui extrair token: {root}");
    }
}