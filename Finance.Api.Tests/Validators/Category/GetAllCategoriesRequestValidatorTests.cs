using Finance.Application.Features.Categories.GetAll;
using Finance.Contracts.Requests.Categories;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Category;

public class GetAllCategoriesRequestValidatorTests
{
    private readonly GetAllCategoriesRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new GetAllCategoriesRequest { PageNumber = 1, PageSize = 15 };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_When_PageNumber_Is_Not_Positive(int invalidPageNumber)
    {
        var model = new GetAllCategoriesRequest { PageNumber = invalidPageNumber, PageSize = 10 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(r => r.PageNumber)
              .WithErrorMessage("O número da página deve ser maior que zero.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_When_PageSize_Is_Not_Positive(int invalidPageSize)
    {
        var model = new GetAllCategoriesRequest { PageNumber = 1, PageSize = invalidPageSize };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(r => r.PageSize)
              .WithErrorMessage("O tamanho da página deve ser maior que zero.");
    }
}