using Finance.Domain.Enums;

namespace Finance.Domain.Models.DTOs;

public class TransactionDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public ETransactionType Type { get; set; }
    public DateTime? PaidOrReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryDto Category { get; set; }
}

