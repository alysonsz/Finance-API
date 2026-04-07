namespace Finance.Contracts.DTOs;

public class TransactionDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidOrReceivedAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public long CategoryId { get; set; }
    public string CategoryTitle { get; set; } = string.Empty;
}
