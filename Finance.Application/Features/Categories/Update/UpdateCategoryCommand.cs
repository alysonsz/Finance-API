using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Categories.Update;

public class UpdateCategoryCommand : IRequest<Response<CategoryDto?>>
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long UserId { get; set; }
}
