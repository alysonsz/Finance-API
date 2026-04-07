using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Categories.GetById;

public class GetCategoryByIdCommand : IRequest<Response<CategoryDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; } 
}
