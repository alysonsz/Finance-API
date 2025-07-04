using Finance.Api.Validators.Auth;
using Finance.Contracts.Requests.Auth;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Auth;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new LoginRequest { Email = "teste@email.com", Password = "Password123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("emailinvalido")]
    [InlineData("email@.com")]
    public void Should_Fail_When_Email_Is_Invalid(string invalidEmail)
    {
        var model = new LoginRequest { Email = invalidEmail, Password = "Password123" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Empty()
    {
        var model = new LoginRequest { Email = "teste@email.com", Password = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}