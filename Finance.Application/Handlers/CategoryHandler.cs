using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;

namespace Finance.Application.Handlers;

public class CategoryHandler(ICategoryRepository repository, ICacheService cacheService) : ICategoryHandler
{
    public async Task<Response<CategoryDto?>> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        };

        try
        {
            await repository.CreateAsync(category);

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto, 201, "Categoria criada com sucesso!");
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, "Não foi possível criar a categoria");
        }
    }

    public async Task<Response<CategoryDto?>> UpdateAsync(UpdateCategoryRequest request)
    {
        return await HandleCategoryOperationAsync(
            request.Id,
            request.UserId,
            "alterar",
            "Categoria atualizada com sucesso",
            async (category) =>
            {
                category.Title = request.Title;
                category.Description = request.Description;
                await repository.UpdateAsync(category);
            });
    }

    public async Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request)
    {
        return await HandleCategoryOperationAsync(
            request.Id,
            request.UserId,
            "excluir",
            "Categoria excluída com sucesso!",
            async (category) => await repository.DeleteAsync(category));
    }

    public async Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        string cacheKey = $"category:{request.Id}:user:{request.UserId}";

        var cachedCategory = await cacheService.GetAsync<CategoryDto>(cacheKey);

        if (cachedCategory is not null)
            return new Response<CategoryDto?>(cachedCategory, 200, "Categoria recuperada do cache");

        var category = await repository.GetByIdAsync(request.Id, request.UserId);

        if (category is null)
            return new Response<CategoryDto?>(null, 404, "Categoria não encontrada");

        var dto = MapToDto(category);

        await cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
        return new Response<CategoryDto?>(dto, 200, "Categoria recuperada com sucesso");

    }

    public async Task<PagedResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        try
        {
            var allCategories = await repository.GetAllAsync(request.UserId);

            if (allCategories == null)
                return new PagedResponse<List<CategoryDto>?>(message: "Não foi possível obter as categorias", code: 500);

            var pagedData = allCategories
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = pagedData.Select(MapToDto).ToList();

            return new PagedResponse<List<CategoryDto>?>(dtos, allCategories.Count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<CategoryDto>?>(message: "Não foi possível consultar as categorias", code: 500);
        }
    }

    private async Task<Response<CategoryDto?>> HandleCategoryOperationAsync(
        long id,
        long userId,
        string actionNameError,
        string successMessage,
        Func<Category, Task> operation)
    {
        try
        {
            var category = await repository.GetByIdAsync(id, userId);

            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada");

            await operation(category);

            await cacheService.RemoveAsync($"category:{id}:user:{userId}");

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto, 200, successMessage);
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, $"Não foi possível {actionNameError} a categoria");
        }
    }

    private static CategoryDto MapToDto(Category category)
        => new()
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description
        };
}