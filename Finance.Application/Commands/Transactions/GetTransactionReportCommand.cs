using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Transactions;
using MediatR;

namespace Finance.Application.Commands.Transactions;

public class GetTransactionReportCommand : IRequest<Response<TransactionReportResponse>>
{
    public long UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
