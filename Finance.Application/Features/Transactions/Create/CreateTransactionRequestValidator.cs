using Finance.Application.Validation;
using Finance.Contracts.Requests.Transactions;
using FluentValidation;

namespace Finance.Application.Features.Transactions.Create;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {
        this.ApplyCommonRules(
            x => x.Title,
            x => x.Amount,
            x => x.Type,
            x => x.CategoryId);
    }
}