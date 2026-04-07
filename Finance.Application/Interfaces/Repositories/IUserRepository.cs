using Finance.Domain.Models;

namespace Finance.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(long id);
    Task<User> UpdateAsync(User u);
}
