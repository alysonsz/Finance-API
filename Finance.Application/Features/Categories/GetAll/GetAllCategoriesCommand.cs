using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Categories.GetAll;


public class GetAllCategoriesCommand : IRequest<PagedResponse<List<CategoryDto>?>>
{
    public long UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
