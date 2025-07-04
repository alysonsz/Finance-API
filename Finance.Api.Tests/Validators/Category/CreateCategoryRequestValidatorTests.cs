using Finance.Api.Validators.Category;
using Finance.Contracts.Requests.Categories;
using FluentValidation.TestHelper;

namespace Finance.Api.Tests.Validators.Category;

public class CreateCategoryRequestValidatorTests
{
    private readonly CreateCategoryRequestValidator _validator = new();

    [Fact]
    public void Should_Succeed_When_Request_Is_Valid()
    { 
        var model = new CreateCategoryRequest { Title = "Categoria Válida", Description = "Descrição Válida" };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var model = new CreateCategoryRequest { Title = string.Empty, Description = "Descrição Válida" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Title)
              .WithErrorMessage("O título é obrigatório."); 
    }

    [Theory]
    [InlineData("a")]  
    [InlineData("ab")] 
    public void Should_Fail_When_Title_Is_Shorter_Than_3_Chars(string invalidTitle)
    {
        var model = new CreateCategoryRequest { Title = invalidTitle, Description = "Descrição Válida" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Title)
              .WithErrorMessage("O título deve ter entre 3 e 80 caracteres.");
    }
}