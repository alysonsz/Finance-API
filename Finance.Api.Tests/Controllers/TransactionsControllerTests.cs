using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Net;

namespace Finance.Api.Tests.Controllers
{
    [Collection("Shared Test Collection")]
    public class TransactionsControllerTests(CustomWebApplicationFactory factory)
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task CreateTransaction_Should_ReturnOk_When_RequestIsValid()
        {
            var request = new CreateTransactionRequest
            {
                Title = "Salário",
                Amount = 5000,
                Type = ETransactionType.Deposit,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now
            };

            var response = await _client.PostAsJsonAsync("v1/transactions", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateTransaction_Should_ReturnOk_When_RequestIsValid()
        {
            var createRequest = new CreateTransactionRequest
            {
                Title = "Transação para Atualizar",
                Amount = 100,
                Type = ETransactionType.Withdraw,
                CategoryId = 1, 
                PaidOrReceivedAt = DateTime.Now
            };
            var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionDto>>();

            createdTransaction.Should().NotBeNull();
            createdTransaction!.Data.Should().NotBeNull();
            var transactionIdToUpdate = createdTransaction.Data.Id;

            var updateRequest = new UpdateTransactionRequest
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
            var createRequest = new CreateTransactionRequest
            {
                Title = "Transação para Deletar",
                Amount = 50,
                Type = ETransactionType.Withdraw,
                CategoryId = 1, 
                PaidOrReceivedAt = DateTime.Now
            };
            var createResponse = await _client.PostAsJsonAsync("v1/transactions", createRequest);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionDto>>();

            createdTransaction.Should().NotBeNull();
            createdTransaction!.Data.Should().NotBeNull();
            var transactionIdToDelete = createdTransaction.Data.Id;

            var deleteResponse = await _client.DeleteAsync($"v1/transactions/{transactionIdToDelete}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByPeriod_Should_ReturnOk_With_PagedData()
        {

            var response = await _client.GetAsync("v1/transactions");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<List<TransactionDto>>>();
            pagedResponse.Should().NotBeNull();
            pagedResponse.Data.Should().NotBeNull();
            pagedResponse.Data.Count.Should().BeGreaterThan(0);
        }
    }
}