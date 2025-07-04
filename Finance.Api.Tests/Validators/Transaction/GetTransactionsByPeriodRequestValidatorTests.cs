using Finance.Api.Validators.Transaction;
using Finance.Contracts.Requests.Transactions;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Transaction;

public class GetTransactionsByPeriodRequestValidatorTests
{
    private readonly GetTransactionsByPeriodRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Dates_Are_Valid()
    {
        var model = new GetTransactionsByPeriodRequest
        {
            StartDate = new DateTime(2025, 7, 1),
            EndDate = new DateTime(2025, 7, 31),
            PageNumber = 1,
            PageSize = 25
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_EndDate_Is_Before_StartDate()
    {
        // Arrange
        var model = new GetTransactionsByPeriodRequest
        {
            StartDate = new DateTime(2025, 7, 31),
            EndDate = new DateTime(2025, 7, 1) // Data final anterior à inicial
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndDate)
              .WithErrorMessage("A data final deve ser posterior ou igual à data inicial.");
    }

    [Fact]
    public void Should_Succeed_When_Only_One_Date_Is_Provided()
    {
        var modelWithOnlyStart = new GetTransactionsByPeriodRequest { StartDate = new DateTime(2025, 7, 1) };
        var modelWithOnlyEnd = new GetTransactionsByPeriodRequest { EndDate = new DateTime(2025, 7, 31) };

        var result1 = _validator.TestValidate(modelWithOnlyStart);
        var result2 = _validator.TestValidate(modelWithOnlyEnd);

        result1.ShouldNotHaveAnyValidationErrors();
        result2.ShouldNotHaveAnyValidationErrors();
    }
}