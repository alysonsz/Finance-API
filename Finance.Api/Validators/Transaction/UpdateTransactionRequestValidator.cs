using Finance.Contracts.Requests.Transactions;
using FluentValidation;

namespace Finance.Api.Validators.Transaction;

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .Length(3, 80).WithMessage("O título deve ter entre 3 e 80 caracteres.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("O valor é obrigatório.")
            .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("O tipo da transação é obrigatório.")
            .IsInEnum().WithMessage("O tipo da transação deve ser 'Deposit' (1) ou 'Withdraw' (2).");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.")
            .GreaterThan(0).WithMessage("O ID da categoria inválido.");
    }
}