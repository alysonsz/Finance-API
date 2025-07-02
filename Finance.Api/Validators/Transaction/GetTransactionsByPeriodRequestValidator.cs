using Finance.Contracts.Requests.Transactions;
using FluentValidation;

namespace Finance.Api.Validators.Transaction;

public class GetTransactionsByPeriodRequestValidator : AbstractValidator<GetTransactionsByPeriodRequest>
{
    public GetTransactionsByPeriodRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("O número da página deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("O tamanho da página deve ser maior que zero.");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("A data final deve ser posterior ou igual à data inicial.");
    }
}