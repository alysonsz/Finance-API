using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Transactions.Delete;

public class DeleteTransactionCommand : IRequest<Response<TransactionDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
}
