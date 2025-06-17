using Finance.Application.Interfaces.Handlers;
using Finance.Application.Requests.Transactions;
using Finance.Application.Responses;
using Finance.Domain.Models;
using System.Net.Http.Json;

namespace Finance.Web.Handlers;

public class TransactionHandler(IHttpClientFactory httpClientFactory) : ITransactionHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/transactions", request);
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
               ?? new Response<Transaction?>(null, 400, "Falha ao criar a transação");
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/transactions/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
               ?? new Response<Transaction?>(null, 400, "Falha ao atualizar a transação");
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        var result = await _client.DeleteAsync($"v1/transactions/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
               ?? new Response<Transaction?>(null, 400, "Falha ao excluir a transação");
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        => await _client.GetFromJsonAsync<Response<Transaction?>>($"v1/transactions/{request.Id}")
           ?? new Response<Transaction?>(null, 400, "Não foi possível obter a transação");

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        var startDate = request.StartDate?.ToString("yyyy-MM-dd") ?? "";
        var endDate = request.EndDate?.ToString("yyyy-MM-dd") ?? "";

        var url = $"v1/transactions?startDate={startDate}&endDate={endDate}&pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        return await _client.GetFromJsonAsync<PagedResponse<List<Transaction>?>>(url)
               ?? new PagedResponse<List<Transaction>?>("Não foi possível obter as transações", 500);
    }
}