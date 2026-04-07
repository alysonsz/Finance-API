namespace Finance.Domain.Models.DTOs;

public class TransactionDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime? PaidOrReceivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryDto? Category { get; set; }
}

