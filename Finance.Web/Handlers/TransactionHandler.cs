using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Transactions;
using Finance.Domain.Models.DTOs;
using System.Net.Http.Json;

namespace Finance.Web.Handlers;

public class TransactionHandler(IHttpClientFactory httpClientFactory) : ITransactionHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

    public async Task<Response<TransactionDto?>> CreateAsync(CreateTransactionRequest request)
    {
        request.Amount = Convert.ToDecimal(request.Amount);

        return await PostAsync<CreateTransactionRequest, TransactionDto?>("v1/transactions", request, "Falha ao criar a transação");
    }

    public async Task<Response<TransactionDto?>> UpdateAsync(UpdateTransactionRequest request)
    {
        request.Amount = Convert.ToDecimal(request.Amount);

        return await PutAsync<UpdateTransactionRequest, TransactionDto?>($"v1/transactions/{request.Id}", request, "Falha ao atualizar a transação");
    }

    public async Task<Response<TransactionDto?>> DeleteAsync(DeleteTransactionRequest request)
        => await DeleteAsync<TransactionDto?>($"v1/transactions/{request.Id}", "Falha ao excluir a transação");

    public async Task<Response<TransactionDto?>> GetByIdAsync(GetTransactionByIdRequest request)
        => await GetAsync<TransactionDto?>($"v1/transactions/{request.Id}", "Não foi possível obter a transação");

    public async Task<PagedResponse<List<TransactionDto>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        var start = request.StartDate?.ToString("yyyy-MM-dd") ?? "";
        var end = request.EndDate?.ToString("yyyy-MM-dd") ?? "";

        var url = $"v1/transactions?startDate={start}&endDate={end}&pageNumber={request.PageNumber}&pageSize={request.PageSize}";

        return await _client.GetFromJsonAsync<PagedResponse<List<TransactionDto>?>>(url)
               ?? new PagedResponse<List<TransactionDto>?>("Não foi possível obter as transações", 500);
    }

    public async Task<Response<TransactionReportResponse>> GetReportAsync(GetTransactionReportRequest request)
    {
        var queryParams = new List<string>();

        if (request.StartDate.HasValue)
            queryParams.Add($"startDate={request.StartDate.Value:yyyy-MM-dd}");

        if (request.EndDate.HasValue)
            queryParams.Add($"endDate={request.EndDate.Value:yyyy-MM-dd}");

        var queryString = queryParams.Count > 0
            ? "?" + string.Join("&", queryParams)
            : string.Empty;

        var url = $"v1/transactions/report{queryString}";

        return await _client.GetFromJsonAsync<Response<TransactionReportResponse>>(url)
               ?? new Response<TransactionReportResponse>(default, 500, "Não foi possível carregar o relatório.");
    }

    private async Task<Response<T?>> GetAsync<T>(string url, string error)
        => await _client.GetFromJsonAsync<Response<T?>>(url) ?? new Response<T?>(default, 500, error);

    private async Task<Response<T?>> PostAsync<TIn, T>(string url, TIn body, string error)
    {
        var res = await _client.PostAsJsonAsync(url, body);
        return await res.Content.ReadFromJsonAsync<Response<T?>>() ?? new Response<T?>(default, 500, error);
    }

    private async Task<Response<T?>> PutAsync<TIn, T>(string url, TIn body, string error)
    {
        var res = await _client.PutAsJsonAsync(url, body);
        return await res.Content.ReadFromJsonAsync<Response<T?>>() ?? new Response<T?>(default, 500, error);
    }

    private async Task<Response<T?>> DeleteAsync<T>(string url, string error)
    {
        var res = await _client.DeleteAsync(url);
        return await res.Content.ReadFromJsonAsync<Response<T?>>() ?? new Response<T?>(default, 500, error);
    }
}
