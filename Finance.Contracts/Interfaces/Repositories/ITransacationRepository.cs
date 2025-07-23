using Finance.Domain.Models;

namespace Finance.Contracts.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> CreateAsync(Transaction transaction);
    Task<Transaction?> UpdateAsync(Transaction transaction);
    Task<Transaction?> DeleteAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, long userId);
    Task<List<Transaction>> GetByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    Task<List<Transaction>> GetAllByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate);
    Task<int> CountByPeriodAsync(long userId, DateTime? startDate, DateTime? endDate);
}