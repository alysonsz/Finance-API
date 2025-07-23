using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Commands.Transactions;

public class GetTransactionByIdCommand : IRequest<Response<TransactionDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
}
