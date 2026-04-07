using Finance.Domain.SeedWork;

namespace Finance.Domain.Models;

public class User : Entity
{
    private User() { }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public ICollection<Category>? Categories { get; private set; }
    public ICollection<Transaction>? Transactions { get; private set; }

    public static Result<User> Create(string name, string email, string passwordHash)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            errors.Add("Nome deve ter pelo menos 2 caracteres");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            errors.Add("Email inválido");

        if (string.IsNullOrWhiteSpace(passwordHash))
            errors.Add("Senha inválida");

        if (errors.Any())
            return Result<User>.Failure(errors);

        var user = new User
        {
            Name = name.Trim(),
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash
        };

        return Result<User>.Success(user);
    }

    public Result<Unit> UpdateProfile(string name, string email)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            errors.Add("Nome deve ter pelo menos 2 caracteres");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            errors.Add("Email inválido");

        if (errors.Any())
            return Result<Unit>.Failure(errors);

        Name = name.Trim();
        Email = email.Trim().ToLower();

        return Result<Unit>.Success(Unit.Value);
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public bool IsRefreshTokenValid(string token)
    {
        return RefreshToken == token && RefreshTokenExpiryTime > DateTime.UtcNow;
    }
}
