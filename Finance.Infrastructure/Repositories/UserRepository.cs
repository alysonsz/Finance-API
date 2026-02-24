using Finance.Contracts.Interfaces.Repositories;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories;

public class UserRepository(FinanceReadDbContext readContext, FinanceWriteDbContext writeContext)
    : BaseRepository<User>(writeContext), IUserRepository
{
    public async Task AddAsync(User user)
    {
        await CreateWithOutboxAsync(user);
    }

    public async Task<User> UpdateAsync(User user)
    {
        await UpdateWithOutboxAsync(user);
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await readContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await readContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
