using Finance.Contracts.Interfaces.Repositories;
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

    public async Task<Transaction?> GetByIdAsync(long id, long userId)
        => await context.Transactions
            .AsNoTracking()
            .Include(c => c.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<List<Transaction>?> GetByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(c => c.Category)
            .Where(t => t.UserId == userId &&
                        (startDate == null || t.PaidOrReceivedAt >= startDate) &&
                        (endDate == null || t.PaidOrReceivedAt <= endDate))
            .OrderBy(t => t.PaidOrReceivedAt)
            .ToListAsync();
    }
}