﻿using Finance.Domain.Enums;

namespace Finance.Domain.Models;

public class Transaction
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? PaidOrReceivedAt { get; set; }

    public ETransactionType Type { get; set; } = ETransactionType.Withdraw;
    public decimal Amount { get; set; }

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public long UserId { get; set; }
    public User? User { get; set; }
}