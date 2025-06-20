using Finance.Domain.Models.DTOs;

namespace Finance.Domain.Models;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public ICollection<Category>? Categories { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
}
