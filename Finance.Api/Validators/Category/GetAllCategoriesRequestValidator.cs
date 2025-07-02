using Finance.Contracts.Requests.Categories;
using FluentValidation;

namespace Finance.Api.Validators.Category;

public class GetAllCategoriesRequestValidator : AbstractValidator<GetAllCategoriesRequest>
{
    public GetAllCategoriesRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("O número da página deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("O tamanho da página deve ser maior que zero.");
    }
}