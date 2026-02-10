using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;

namespace Finance.Application.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(Transaction transaction, Category category)
        => new()
        {
            Id = transaction.Id,
            Title = transaction.Title,
            Amount = transaction.Amount,
            Type = transaction.Type,
            PaidOrReceivedAt = transaction.PaidOrReceivedAt,
            CreatedAt = transaction.CreatedAt,
            Category = new CategoryDto
            {
                Id = category.Id,
                Title = category.Title,
                Description = category.Description
            }
        };
}
