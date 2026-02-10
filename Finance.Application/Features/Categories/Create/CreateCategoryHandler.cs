using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Categories.Create;

public class CreateCategoryHandler(ICategoryRepository repository) : IRequestHandler<CreateCategoryCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
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

    private static CategoryDto MapToDto(Category category)
        => new()
        {
            Id = category.Id,
            Title = category.Title,
            Description = category.Description
        };
}
