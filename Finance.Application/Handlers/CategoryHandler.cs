using Finance.Application.Interfaces.Handlers;
using Finance.Application.Interfaces.Repositories;
using Finance.Application.Requests.Categories;
using Finance.Application.Responses;
using Finance.Domain.Models;

namespace Finance.Application.Handlers;

public class CategoryHandler(ICategoryRepository repository) : ICategoryHandler
{
    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category { UserId = request.UserId, Title = request.Title, Description = request.Description };
        try
        {
            await repository.CreateAsync(category);
            return new Response<Category?>(category, 201, "Categoria criada com sucesso!");
        }
        catch { return new Response<Category?>(null, 500, "Não foi possível criar a categoria"); }
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null) return new Response<Category?>(null, 404, "Categoria não encontrada");

            category.Title = request.Title;
            category.Description = request.Description;
            await repository.UpdateAsync(category);

            return new Response<Category?>(category, 200, "Categoria atualizada com sucesso");
        }
        catch { return new Response<Category?>(null, 500, "Não foi possível alterar a categoria"); }
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null) return new Response<Category?>(null, 404, "Categoria não encontrada");

            await repository.DeleteAsync(category);
            return new Response<Category?>(category, 200, "Categoria excluída com sucesso!");
        }
        catch { return new Response<Category?>(null, 500, "Não foi possível excluir a categoria"); }
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            return category is null
                ? new Response<Category?>(null, 404, "Categoria não encontrada")
                : new Response<Category?>(category);
        }
        catch { return new Response<Category?>(null, 500, "Não foi possível recuperar a categoria"); }
    }

    public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        try
        {
            var allCategories = await repository.GetAllAsync(request.UserId);

            if (allCategories == null)
            {
                return new PagedResponse<List<Category>?>("Não foi possível obter as categorias", 500);
            }

            var pagedData = allCategories.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<Category>?>(pagedData, allCategories.Count, request.PageNumber, request.PageSize);
        }
        catch { return new PagedResponse<List<Category>?>("Não foi possível consultar as categorias", 500); }
    }
}