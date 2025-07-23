using Finance.Application.Commands.Categories;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Handlers.Categories;

public class GetAllCategoriesCommandHandler(ICategoryRepository repository) : IRequestHandler<GetAllCategoriesCommand, PagedResponse<List<CategoryDto>?>>
{
    public async Task<PagedResponse<List<CategoryDto>?>> Handle(GetAllCategoriesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allCategories = await repository.GetAllAsync(request.UserId);

            if (allCategories is null)
                return new PagedResponse<List<CategoryDto>?>("Não foi possível obter as categorias.", 500);

            var pagedData = allCategories
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(MapToDto)
                .ToList();

            return new PagedResponse<List<CategoryDto>?>(
                pagedData,
                allCategories.Count,
                request.PageNumber,
                request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<CategoryDto>?>("Ocorreu um erro ao consultar as categorias.", 500);
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
