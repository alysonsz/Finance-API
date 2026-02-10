using Finance.Contracts.Requests.Categories;
using FluentValidation;

namespace Finance.Application.Features.Categories.Create;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .Length(3, 80).WithMessage("O título deve ter entre 3 e 80 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(255).WithMessage("A descrição deve ter no máximo 255 caracteres.");
    }
}