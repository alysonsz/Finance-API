using Finance.Domain.Enums;

namespace Finance.Contracts.Responses.Transactions;

public sealed class TransactionOutboxResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ETransactionType Type { get; set; }
    public DateTime? PaidOrReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CategoryId { get; set; }
    public long UserId { get; set; }
}
