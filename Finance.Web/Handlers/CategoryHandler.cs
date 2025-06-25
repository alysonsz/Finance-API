using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using System.Net.Http.Json;

namespace Finance.Web.Handlers;

public class CategoryHandler(IHttpClientFactory httpClientFactory) : ICategoryHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

    public async Task<Response<CategoryDto?>> CreateAsync(CreateCategoryRequest request)
        => await PostAsync<CreateCategoryRequest, CategoryDto?>("v1/categories", request, "Falha ao criar categoria");

    public async Task<Response<CategoryDto?>> UpdateAsync(UpdateCategoryRequest request)
        => await PutAsync<UpdateCategoryRequest, CategoryDto?>($"v1/categories/{request.Id}", request, "Falha ao atualizar a categoria");

    public async Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request)
        => await DeleteAsync<CategoryDto?>($"v1/categories/{request.Id}", "Falha ao excluir a categoria");

    public async Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request)
        => await GetAsync<CategoryDto?>($"v1/categories/{request.Id}", "Não foi possível obter a categoria");

    public async Task<PagedResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        var url = $"v1/categories?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        return await _client.GetFromJsonAsync<PagedResponse<List<CategoryDto>?>>(url)
               ?? new PagedResponse<List<CategoryDto>?>("Não foi possível obter as categorias", 500);
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
