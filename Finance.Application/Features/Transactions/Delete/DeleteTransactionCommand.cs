using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.Delete;

public class DeleteTransactionCommand : IRequest<Response<TransactionDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
}
