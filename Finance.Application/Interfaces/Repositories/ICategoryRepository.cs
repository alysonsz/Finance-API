using Finance.Domain.Models;

namespace Finance.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> CreateAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task<Category?> DeleteAsync(Category category);
    Task<Category?> GetByIdAsync(long id, long userId);
    Task<List<Category>?> GetAllAsync(long userId);
}