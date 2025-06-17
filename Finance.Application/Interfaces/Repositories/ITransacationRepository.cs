using Finance.Domain.Models;

namespace Finance.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> CreateAsync(Transaction transaction);
    Task<Transaction?> UpdateAsync(Transaction transaction);
    Task<Transaction?> DeleteAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, string userId);
    Task<List<Transaction>?> GetByPeriodAsync(string userId, DateTime? startDate, DateTime? endDate);
}