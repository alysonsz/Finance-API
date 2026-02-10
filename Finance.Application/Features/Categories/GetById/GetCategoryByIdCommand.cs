using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Categories.GetById;

public class GetCategoryByIdCommand : IRequest<Response<CategoryDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; } 
}
