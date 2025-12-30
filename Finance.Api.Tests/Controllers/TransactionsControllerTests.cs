using Finance.Application.Commands.Transactions;
using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Finance.Api.Tests.Controllers
{
    [Collection("Shared Test Collection")]
    public class TransactionsControllerTests(CustomWebApplicationFactory factory)
    {
        private readonly HttpClient _client = factory.CreateClient();
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        [Fact]
        public async Task CreateTransaction_Should_ReturnOk_When_RequestIsValid()
        {
            var request = new CreateTransactionCommand
            {
                Title = "Salário",
                Amount = 5000,
                Type = ETransactionType.Deposit,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now
            };

            var response = await _client.PostAsJsonAsync("v1/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateTransaction_Should_ReturnOk_When_RequestIsValid()
        {
            var createRequest = new CreateTransactionCommand
            {
                Title = "Transação para Atualizar",
                Amount = 100,
                Type = ETransactionType.Withdraw,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now
            };
            var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);

            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<TransactionDto>(_options);

            createdTransaction.Should().NotBeNull();

            var transactionIdToUpdate = createdTransaction.Id;

            var updateRequest = new UpdateTransactionCommand
            {
                Id = transactionIdToUpdate,
                Title = "Jantar (Editado)",
                Amount = 180,
                Type = ETransactionType.Withdraw,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now
            };

            var response = await _client.PutAsJsonAsync($"v1/transactions/{transactionIdToUpdate}", updateRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteTransaction_Should_ReturnOk_When_TransactionExists()
        {
            var createRequest = new CreateTransactionCommand
            {
                Title = "Transação para Deletar",
                Amount = 50,
                Type = ETransactionType.Withdraw,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now
            };
            var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);

            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<TransactionDto>(_options);

            createdTransaction.Should().NotBeNull();

            var transactionIdToDelete = createdTransaction.Id;

            var deleteResponse = await _client.DeleteAsync($"v1/transactions/{transactionIdToDelete}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByPeriod_Should_ReturnOk_With_PagedData()
        {

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
}