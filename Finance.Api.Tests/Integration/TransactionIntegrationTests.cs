using Finance.Contracts.Requests.Transactions;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Text.Json;

namespace Finance.Api.Tests.Integration;

public class TransactionIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_Should_SaveToDatabase_And_BeRetrievable()
    {
        var email = $"test-{Guid.NewGuid()}@integration.com";
        var password = "Password123!";

        var registerRes = await _client.PostAsJsonAsync("/v1/auth/register", new
        {
            Name = "Tester",
            Email = email,
            Password = password
        });
        registerRes.EnsureSuccessStatusCode();

        var loginRes = await _client.PostAsJsonAsync("/v1/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginRes.EnsureSuccessStatusCode();

        var loginJson = await loginRes.Content.ReadFromJsonAsync<JsonElement>();
        var token = ExtractAccessToken(loginJson);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var catRes = await _client.PostAsJsonAsync("/v1/categories", new { Title = "Test Cat", Description = "Desc" });
        catRes.EnsureSuccessStatusCode();

        var cat = await catRes.Content.ReadFromJsonAsync<CategoryDto>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        cat.Should().NotBeNull();
        var catId = cat!.Id;

        var request = new CreateTransactionRequest
        {
            Title = "Integration Test Transaction",
            Amount = 100.50m,
            Type = ETransactionType.Withdraw,
            CategoryId = catId,
            PaidOrReceivedAt = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/v1/transactions", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<TransactionDto>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.Title.Should().Be(request.Title);
        result.Id.Should().BeGreaterThan(0);
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