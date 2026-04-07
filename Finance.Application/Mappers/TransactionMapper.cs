using Finance.Contracts.DTOs;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(Transaction transaction, Category category)
        => new()
        {
            Id = transaction.Id,
            Title = transaction.Title,
            Amount = transaction.Amount,
            Type = transaction.Type.ToString(),
            PaidOrReceivedAt = transaction.PaidOrReceivedAt,
            CreatedAt = transaction.CreatedAt,
            CategoryTitle = category.Title
        };
}
