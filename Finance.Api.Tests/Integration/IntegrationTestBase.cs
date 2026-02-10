using Finance.Contracts.Requests.Auth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Finance.Api.Tests.Integration;

public class IntegrationTestBase(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client = factory.CreateClient();

    protected async Task AuthenticateAsync()
    {
        if (_client.DefaultRequestHeaders.Authorization != null)
            return;

        var email = $"integration-{Guid.NewGuid()}@tests.com";
        var password = "Password123!";

        var registerRes = await _client.PostAsJsonAsync("/v1/auth/register", new RegisterRequest
        {
            Name = "Integration User",
            Email = email,
            Password = password
        });
        registerRes.EnsureSuccessStatusCode();

        var loginRes = await _client.PostAsJsonAsync("/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });
        loginRes.EnsureSuccessStatusCode();

        var root = await loginRes.Content.ReadFromJsonAsync<JsonElement>();
        var token = ExtractAccessToken(root);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private static string ExtractAccessToken(JsonElement root)
    {
        if (root.TryGetProperty("token", out var tokenEl))
        {
            if (tokenEl.ValueKind == JsonValueKind.String)
                return tokenEl.GetString() ?? throw new InvalidOperationException("Token string veio null.");

            if (tokenEl.ValueKind == JsonValueKind.Object && tokenEl.TryGetProperty("accessToken", out var accessTokenEl))
                return accessTokenEl.GetString() ?? throw new InvalidOperationException("accessToken veio null.");
        }

        if (root.TryGetProperty("accessToken", out var at))
            return at.GetString() ?? throw new InvalidOperationException("accessToken veio null (root).");

        throw new InvalidOperationException($"Não consegui extrair token do payload: {root}");
    }
}