using Finance.Application.Commands.Categories;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Handlers.Categories;

public class GetCategoryByIdHandler(ICategoryRepository repository)
    : IRequestHandler<GetCategoryByIdCommand, Response<CategoryDto?>>
{
    public async Task<Response<CategoryDto?>> Handle(GetCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await repository.GetByIdAsync(request.Id, request.UserId);

            if (category is null)
                return new Response<CategoryDto?>(null, 404, "Categoria não encontrada ou não pertence ao usuário.");

            var dto = MapToDto(category);
            return new Response<CategoryDto?>(dto); 
        }
        catch
        {
            return new Response<CategoryDto?>(null, 500, "Não foi possível recuperar a categoria.");
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
