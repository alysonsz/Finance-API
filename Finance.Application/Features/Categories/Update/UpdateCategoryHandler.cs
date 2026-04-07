using Finance.Application.Interfaces.Repositories;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Categories.Update;

public class UpdateCategoryHandler(ICategoryRepository repository) : IRequestHandler<UpdateCategoryCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);
            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada ou não pertence ao usuário.");

            var updateResult = category.Update(request.Title, request.Description);
            if (updateResult.IsFailure)
                return Response<CategoryDto?>.Fail(string.Join("; ", updateResult.Errors));

            await repository.UpdateAsync(category);

            var dto = MapToDto(category);
            return Response<CategoryDto?>.Success(dto, "Categoria atualizada com sucesso");
        }
        catch
        {
            return Response<CategoryDto?>.Fail("Não foi possível alterar a categoria");
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
