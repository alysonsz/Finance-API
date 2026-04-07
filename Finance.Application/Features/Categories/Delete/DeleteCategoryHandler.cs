using Finance.Application.Interfaces.Repositories;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Categories.Delete;

public class DeleteCategoryHandler(ICategoryRepository repository) : IRequestHandler<DeleteCategoryCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);

            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada ou não pertence ao usuário.");

            await repository.DeleteAsync(category);

            var dto = MapToDto(category);
            return Response<CategoryDto?>.Success(dto, "Categoria excluída com sucesso!");
        }
        catch
        {
            return Response<CategoryDto?>.Fail("Não foi possível excluir a categoria");
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
