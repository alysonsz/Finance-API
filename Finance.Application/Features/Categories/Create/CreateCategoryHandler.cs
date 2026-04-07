using Finance.Application.Interfaces.Repositories;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Categories.Create;

public class CreateCategoryHandler(ICategoryRepository repository) : IRequestHandler<CreateCategoryCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = Category.Create(request.Title, request.Description, request.UserId);
        if (result.IsFailure)
            return Response<CategoryDto?>.Fail(string.Join("; ", result.Errors));

        var category = result.Value;

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
