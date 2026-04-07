using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Categories.Delete;

public class DeleteCategoryCommand : IRequest<Response<CategoryDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
}
