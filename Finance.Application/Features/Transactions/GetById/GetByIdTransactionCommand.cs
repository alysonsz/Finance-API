using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using MediatR;

namespace Finance.Application.Features.Transactions.GetById;

public class GetByIdTransactionCommand : IRequest<Response<TransactionDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
}
