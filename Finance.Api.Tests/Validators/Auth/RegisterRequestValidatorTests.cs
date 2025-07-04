using Finance.Api.Validators.Auth;
using Finance.Contracts.Requests.Auth;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Auth;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new RegisterRequest { Name = "Usuário Teste", Email = "teste@email.com", Password = "Password@123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        var model = new RegisterRequest { Name = "", Email = "teste@email.com", Password = "Password@123" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var model = new RegisterRequest { Name = "Usuário Teste", Email = "email-invalido", Password = "Password@123" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("pass", "A senha deve ter no mínimo 8 caracteres.")]
    [InlineData("password123", "A senha deve conter pelo menos uma letra maiúscula.")]
    [InlineData("PASSWORD123", "A senha deve conter pelo menos uma letra minúscula.")]
    [InlineData("PasswordAbc", "A senha deve conter pelo menos um número.")]
    [InlineData("Password123", "A senha deve conter pelo menos um caractere especial.")]
    public void Should_Fail_When_Password_Is_Weak(string weakPassword, string expectedMessage)
    {
        var model = new RegisterRequest { Name = "Usuário Teste", Email = "teste@email.com", Password = weakPassword };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage(expectedMessage);
    }
}