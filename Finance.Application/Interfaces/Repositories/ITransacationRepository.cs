using Finance.Domain.Models;

namespace Finance.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> CreateAsync(Transaction transaction);
    Task<Transaction?> UpdateAsync(Transaction transaction);
    Task<Transaction?> DeleteAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, long userId);
    Task<List<Transaction>?> GetByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate);
}