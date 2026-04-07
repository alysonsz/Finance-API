using Finance.Domain.Enums;
using Finance.Domain.SeedWork;

namespace Finance.Domain.Models;

public class Transaction : Entity, IAggregateRoot
{
    private Transaction() { }

    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidOrReceivedAt { get; private set; }
    public ETransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public long CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public long UserId { get; private set; }
    public User? User { get; private set; }

    public static Result<Transaction> Create(
        string title,
        decimal amount,
        ETransactionType type,
        long categoryId,
        long userId,
        DateTime? paidOrReceivedAt)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            errors.Add("Título deve ter pelo menos 3 caracteres");

        if (amount <= 0)
            errors.Add("Valor deve ser maior que zero");

        if (categoryId <= 0)
            errors.Add("Categoria inválida");

        if (userId <= 0)
            errors.Add("Usuário inválido");

        if (errors.Any())
            return Result<Transaction>.Failure(errors);

        var normalizedAmount = type == ETransactionType.Withdraw && amount > 0
            ? -amount
            : amount;

        var transaction = new Transaction
        {
            Title = title.Trim(),
            Amount = normalizedAmount,
            Type = type,
            CategoryId = categoryId,
            UserId = userId,
            PaidOrReceivedAt = paidOrReceivedAt,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Transaction>.Success(transaction);
    }

    public Result<Unit> Update(
        string title,
        decimal amount,
        ETransactionType type,
        long categoryId,
        DateTime? paidOrReceivedAt)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            errors.Add("Título deve ter pelo menos 3 caracteres");

        if (amount <= 0)
            errors.Add("Valor deve ser maior que zero");

        if (categoryId <= 0)
            errors.Add("Categoria inválida");

        if (errors.Any())
            return Result<Unit>.Failure(errors);

        Title = title.Trim();
        Type = type;
        CategoryId = categoryId;
        PaidOrReceivedAt = paidOrReceivedAt;

        var normalizedAmount = type == ETransactionType.Withdraw && amount > 0
            ? -amount
            : amount;

        Amount = normalizedAmount;

        return Result<Unit>.Success(Unit.Value);
    }

    public bool IsIncome() => Type == ETransactionType.Deposit;
    public bool IsExpense() => Type == ETransactionType.Withdraw;
    public decimal GetAbsoluteAmount() => Math.Abs(Amount);
}