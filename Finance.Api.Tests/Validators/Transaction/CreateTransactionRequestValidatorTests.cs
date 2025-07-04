using Finance.Api.Validators.Transaction;
using Finance.Contracts.Requests.Transactions;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Transaction;

public class CreateTransactionRequestValidatorTests
{
    private readonly CreateTransactionRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new CreateTransactionRequest
        {
            Title = "Salário",
            Amount = 3000,
            Type = ETransactionType.Deposit,
            CategoryId = 1
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var model = new CreateTransactionRequest { Title = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-150.0)]
    public void Should_Fail_When_Amount_Is_Not_Positive(decimal invalidAmount)
    {
        var model = new CreateTransactionRequest { Amount = invalidAmount };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Amount)
              .WithErrorMessage("O valor da transação deve ser maior que zero.");
    }

    [Fact]
    public void Should_Fail_When_Type_Is_Invalid()
    {
        var model = new CreateTransactionRequest { Type = (ETransactionType)99 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Type)
              .WithErrorMessage("O tipo da transação deve ser 'Deposit' (1) ou 'Withdraw' (2).");
    }

    [Fact]
    public void Should_Fail_When_CategoryId_Is_Invalid()
    {
        var model = new CreateTransactionRequest { CategoryId = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}