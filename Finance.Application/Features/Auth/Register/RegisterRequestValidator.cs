using Finance.Contracts.Requests.Auth;
using FluentValidation;

namespace Finance.Application.Features.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("O formato do e-mail é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.");
    }
}