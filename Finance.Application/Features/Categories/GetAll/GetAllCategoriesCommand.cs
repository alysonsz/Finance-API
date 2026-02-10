using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Categories.GetAll;


public class GetAllCategoriesCommand : IRequest<PagedResponse<List<CategoryDto>?>>
{
    public long UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
