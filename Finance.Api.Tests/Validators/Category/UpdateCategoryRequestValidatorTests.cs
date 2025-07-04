using Finance.Api.Validators.Category;
using Finance.Contracts.Requests.Categories;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Category;

public class UpdateCategoryRequestValidatorTests
{
    private readonly UpdateCategoryRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    {
        var model = new UpdateCategoryRequest { Title = "Título Válido", Description = "Descrição Válida" };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var model = new UpdateCategoryRequest { Title = string.Empty, Description = "Descrição Válida" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Title)
              .WithErrorMessage("O título é obrigatório.");
    }

    [Fact]
    public void Should_Fail_When_Description_Is_Empty()
    {
        var model = new UpdateCategoryRequest { Title = "Título Válido", Description = string.Empty };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Description)
              .WithErrorMessage("A descrição é obrigatória.");
    }
}