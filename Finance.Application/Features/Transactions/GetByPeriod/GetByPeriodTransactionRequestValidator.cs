using Finance.Contracts.Requests.Transactions;
using FluentValidation;

namespace Finance.Application.Features.Transactions.GetByPeriod;

public class GetByPeriodTransactionRequestValidator : AbstractValidator<GetTransactionsByPeriodRequest>
{
    public GetByPeriodTransactionRequestValidator()
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