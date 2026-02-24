using Finance.Contracts.Interfaces.Repositories;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

public class TransactionRepository(FinanceReadDbContext readContext, FinanceWriteDbContext writeContext)
    : BaseRepository<Transaction>(writeContext), ITransactionRepository
{
    public async Task<Transaction?> CreateAsync(Transaction transaction)
    {
        await CreateWithOutboxAsync(transaction);
        return transaction;
    }

    public async Task<Transaction?> UpdateAsync(Transaction transaction)
    {
        await UpdateWithOutboxAsync(transaction);
        return transaction;
    }

    public async Task<Transaction?> DeleteAsync(Transaction transaction)
    {
        await DeleteWithOutboxAsync(transaction);
        return transaction;
    }

    public async Task<Transaction?> GetByIdAsync(long id, long userId)
        => await readContext.Transactions
            .AsNoTracking()
            .Include(c => c.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<List<Transaction>> GetByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
    {
        return await readContext.Transactions
            .AsNoTracking()
            .Include(c => c.Category)
            .Where(t => t.UserId == userId &&
                        (startDate == null || t.PaidOrReceivedAt >= startDate) &&
                        (endDate == null || t.PaidOrReceivedAt <= endDate))
            .OrderBy(t => t.PaidOrReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetAllByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate)
    {
        return await readContext.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId &&
                        (startDate == null || t.PaidOrReceivedAt >= startDate) &&
                        (endDate == null || t.PaidOrReceivedAt <= endDate))
            .OrderBy(t => t.PaidOrReceivedAt)
            .ToListAsync();
    }

    public async Task<int> CountByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate)
    {
        return await readContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        (startDate == null || t.PaidOrReceivedAt >= startDate) &&
                        (endDate == null || t.PaidOrReceivedAt <= endDate))
            .CountAsync();
    }
}