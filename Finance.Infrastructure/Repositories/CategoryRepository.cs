using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

public class CategoryRepository(FinanceDbContext context) : ICategoryRepository
{
    public async Task<Category?> CreateAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> DeleteAsync(Category category)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> GetByIdAsync(long id, string userId)
        => await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    public async Task<List<Category>?> GetAllAsync(string userId)
        => await context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Title)
            .ToListAsync();
}