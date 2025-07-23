using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Commands.Categories;

public class CreateCategoryCommand : IRequest<Response<CategoryDto?>>
{
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
