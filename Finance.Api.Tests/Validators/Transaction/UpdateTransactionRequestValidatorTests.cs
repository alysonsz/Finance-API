using Finance.Api.Validators.Transaction;
using Finance.Contracts.Requests.Transactions;
using Finance.Domain.Enums;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Transaction;

public class UpdateTransactionRequestValidatorTests
{
    private readonly UpdateTransactionRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new UpdateTransactionRequest
        {
            Title = "Aluguel",
            Amount = 1500,
            Type = ETransactionType.Withdraw,
            CategoryId = 2
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var model = new UpdateTransactionRequest { Title = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}