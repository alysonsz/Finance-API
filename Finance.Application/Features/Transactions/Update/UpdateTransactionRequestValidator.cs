using Finance.Application.Validation;
using Finance.Contracts.Requests.Transactions;
using FluentValidation;

namespace Finance.Application.Features.Transactions.Update;

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        this.ApplyCommonRules(
            x => x.Title,
            x => x.Amount,
            x => x.Type,
            x => x.CategoryId);
    }
}