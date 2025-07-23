using Finance.Application.Commands.Categories;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Handlers.Categories;

public class UpdateCategoryHandler(ICategoryRepository repository)
    : IRequestHandler<UpdateCategoryCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada ou não pertence ao usuário.");

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

    private static CategoryDto MapToDto(Category category)
        => new()
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description
        };
}
