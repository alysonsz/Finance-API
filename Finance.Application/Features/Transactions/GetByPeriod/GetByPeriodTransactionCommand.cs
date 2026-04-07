using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Transactions.GetByPeriod;

public class GetByPeriodTransactionCommand : IRequest<PagedResponse<List<TransactionDto>?>>
{
    public long UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
