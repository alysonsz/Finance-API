using Finance.Domain.SeedWork;

namespace Finance.Domain.Models;

public class Category : Entity
{
    private Category() { }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public static Result<Category> Create(string title, string? description, long userId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            errors.Add("Título deve ter pelo menos 3 caracteres");

        if (title.Length > 100)
            errors.Add("Título deve ter no máximo 100 caracteres");

        if (userId <= 0)
            errors.Add("Usuário inválido");

        if (errors.Any())
            return Result<Category>.Failure(errors);

        var category = new Category
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            UserId = userId
        };

        return Result<Category>.Success(category);
    }

    public Result<Unit> Update(string title, string? description)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            errors.Add("Título deve ter pelo menos 3 caracteres");

        if (title.Length > 100)
            errors.Add("Título deve ter no máximo 100 caracteres");

        if (errors.Any())
            return Result<Unit>.Failure(errors);

        Title = title.Trim();
        Description = description?.Trim();

        return Result<Unit>.Success(Unit.Value);
    }
}