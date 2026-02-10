using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.Update;

public class UpdateTransactionCommand : IRequest<Response<TransactionDto?>>
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ETransactionType Type { get; set; } = ETransactionType.Withdraw;
    public decimal Amount { get; set; }
    public long CategoryId { get; set; }
    public DateTime? PaidOrReceivedAt { get; set; }
}
