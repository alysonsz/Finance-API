using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using Finance.Application.Features.Categories.Create;
using Finance.Application.Features.Categories.Update;
using Finance.Application.Features.Categories.Delete;
using Finance.Application.Features.Categories.GetById;
using Finance.Application.Features.Categories.GetAll;
using MediatR;

namespace Finance.Application.Services;

public sealed class CategoryService(IMediator mediator) : ICategoryService
{
    private readonly IMediator _mediator = mediator;

    public Task<Response<CategoryDto?>> CreateAsync(CreateCategoryRequest request)
        => _mediator.Send(new CreateCategoryCommand
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        });

    public Task<Response<CategoryDto?>> UpdateAsync(UpdateCategoryRequest request)
        => _mediator.Send(new UpdateCategoryCommand
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId
        });

    public Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request)
        => _mediator.Send(new DeleteCategoryCommand
        {
            Id = request.Id,
            UserId = request.UserId
        });

    public Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request)
        => _mediator.Send(new GetCategoryByIdCommand
        {
            Id = request.Id,
            UserId = request.UserId
        });

    public Task<PagedResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request)
        => _mediator.Send(new GetAllCategoriesCommand
        {
            UserId = request.UserId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
}
