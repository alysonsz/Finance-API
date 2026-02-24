using Finance.Contracts.Interfaces.Repositories;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

public class CategoryRepository(FinanceReadDbContext readContext, FinanceWriteDbContext writeContext)
    : BaseRepository<Category>(writeContext), ICategoryRepository
{
    public async Task<Category?> CreateAsync(Category category)
    {
        await CreateWithOutboxAsync(category);
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        await UpdateWithOutboxAsync(category);
        return category;
    }

    public async Task<Category?> DeleteAsync(Category category)
    {
        await DeleteWithOutboxAsync(category);
        return category;
    }

    public async Task<Category?> GetByIdAsync(long id, long userId)
        => await readContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    public async Task<List<Category>?> GetAllAsync(long userId)
        => await readContext.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Title)
            .ToListAsync();
}