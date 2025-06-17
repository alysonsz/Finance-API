using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

public class TransactionRepository(FinanceDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> CreateAsync(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction?> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction?> DeleteAsync(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction?> GetByIdAsync(long id, string userId)
        => await context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<List<Transaction>?> GetByPeriodAsync(string userId, DateTime? startDate, DateTime? endDate)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.PaidOrReceivedAt >= startDate &&
                        t.PaidOrReceivedAt <= endDate)
            .OrderBy(t => t.PaidOrReceivedAt)
            .ToListAsync();
    }
}