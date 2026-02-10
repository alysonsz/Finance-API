using Finance.Api.Tests.Integration;
using Finance.Application.Features.Transactions.Create;
using Finance.Application.Features.Transactions.Update;
using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Finance.Api.Tests.Controllers;

[Collection("Shared Test Collection")]
public class TransactionsControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task CreateTransaction_Should_ReturnOk_When_RequestIsValid()
    {
        await AuthenticateAsync();

        var catRes = await _client.PostAsJsonAsync("/v1/categories", new 
        {
            Title = "Test Cat", 
            Description = "Desc" 
        });
        catRes.EnsureSuccessStatusCode();

        var cat = await catRes.Content.ReadFromJsonAsync<CategoryDto>(_options);
        cat.Should().NotBeNull();

        var request = new CreateTransactionRequest
        {
            Title = "Test Transaction",
            Amount = 100,
            Type = ETransactionType.Withdraw,
            CategoryId = cat!.Id,
            PaidOrReceivedAt = DateTime.Now
        };

        var response = await _client.PostAsJsonAsync("/v1/transactions", request);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {content}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateTransaction_Should_ReturnOk_When_RequestIsValid()
    {
        await AuthenticateAsync();

        var catRes = await _client.PostAsJsonAsync("/v1/categories", new 
        { 
            Title = "Cat Update Tx", 
            Description = "Desc" 
        });
        catRes.EnsureSuccessStatusCode();

        var cat = await catRes.Content.ReadFromJsonAsync<CategoryDto>(_options);
        cat.Should().NotBeNull();

        var createRequest = new CreateTransactionCommand
        {
            Title = "Transação para Atualizar",
            Amount = 100,
            Type = ETransactionType.Withdraw,
            CategoryId = cat!.Id,
            PaidOrReceivedAt = DateTime.Now
        };

        var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdTransaction = await createResponse.Content.ReadFromJsonAsync<TransactionDto>(_options);
        createdTransaction.Should().NotBeNull();

        var updateRequest = new UpdateTransactionCommand
        {
            Id = createdTransaction!.Id,
            Title = "Jantar (Editado)",
            Amount = 180,
            Type = ETransactionType.Withdraw,
            CategoryId = cat.Id,
            PaidOrReceivedAt = DateTime.Now
        };

        var response = await _client.PutAsJsonAsync($"v1/transactions/{createdTransaction.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTransaction_Should_ReturnOk_When_TransactionExists()
    {
        await AuthenticateAsync();

        var catRes = await _client.PostAsJsonAsync("/v1/categories", new 
        { 
            Title = "Cat Delete Tx", 
            Description = "Desc" 
        });
        catRes.EnsureSuccessStatusCode();

        var cat = await catRes.Content.ReadFromJsonAsync<CategoryDto>(_options);
        cat.Should().NotBeNull();

        var createRequest = new CreateTransactionCommand
        {
            Title = "Transação para Deletar",
            Amount = 50,
            Type = ETransactionType.Withdraw,
            CategoryId = cat!.Id,
            PaidOrReceivedAt = DateTime.Now
        };

        var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdTransaction = await createResponse.Content.ReadFromJsonAsync<TransactionDto>(_options);
        createdTransaction.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"v1/transactions/{createdTransaction!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByPeriod_Should_ReturnOk_With_PagedData()
    {
        await AuthenticateAsync();

        var catRes = await _client.PostAsJsonAsync("/v1/categories", new 
        { 
            Title = "Cat Period Tx", 
            Description = "Desc" 
        });
        catRes.EnsureSuccessStatusCode();

        var cat = await catRes.Content.ReadFromJsonAsync<CategoryDto>(_options);

        await _client.PostAsJsonAsync("/v1/transactions", new CreateTransactionCommand
        {
            Title = $"Seed Tx {Guid.NewGuid()}",
            Amount = 10,
            Type = ETransactionType.Withdraw,
            CategoryId = cat!.Id,
            PaidOrReceivedAt = DateTime.Now
        });

        var response = await _client.GetAsync("v1/transactions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(_options);

        if (content.ValueKind == JsonValueKind.Array)
        {
            content.GetArrayLength().Should().BeGreaterThan(0);
        }
        else
        {
            content.GetProperty("data").GetArrayLength().Should().BeGreaterThan(0);
        }
    }
}