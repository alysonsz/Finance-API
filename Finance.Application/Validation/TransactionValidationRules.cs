using Finance.Domain.Enums;
using FluentValidation;
using System.Linq.Expressions;

namespace Finance.Application.Validation;

public static class TransactionValidationRules
{
    public static void ApplyCommonRules<T>(
        this AbstractValidator<T> validator,
        Expression<Func<T, string>> title,
        Expression<Func<T, decimal>> amount,
        Expression<Func<T, ETransactionType>> type,
        Expression<Func<T, long>> categoryId)
    {
        validator.RuleFor(title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .Length(3, 80).WithMessage("O título deve ter entre 3 e 80 caracteres.");

        validator.RuleFor(amount)
            .NotEmpty().WithMessage("O valor é obrigatório.")
            .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.");

        validator.RuleFor(type)
            .NotEmpty().WithMessage("O tipo da transação é obrigatório.")
            .IsInEnum().WithMessage("O tipo da transação deve ser 'Deposit' (1) ou 'Withdraw' (2).");

        validator.RuleFor(categoryId)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.")
            .GreaterThan(0).WithMessage("O ID da categoria inválido.");
    }
}
