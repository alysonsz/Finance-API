using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;

namespace Finance.Application.Handlers;

public class CategoryHandler(ICategoryRepository repository) : ICategoryHandler
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
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada");

            category.Title = request.Title;
            category.Description = request.Description;

            await repository.UpdateAsync(category);

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto, 200, "Categoria atualizada com sucesso");
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, "Não foi possível alterar a categoria");
        }
    }

    public async Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada");

            await repository.DeleteAsync(category);

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto, 200, "Categoria excluída com sucesso!");
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, "Não foi possível excluir a categoria");
        }
    }

    public async Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada");

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto);
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, "Não foi possível recuperar a categoria");
        }
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
                .Select(MapToDto)
                .ToList();

            return new PagedResponse<List<CategoryDto>?>(pagedData, allCategories.Count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<CategoryDto>?>(message: "Não foi possível consultar as categorias", code: 500);
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