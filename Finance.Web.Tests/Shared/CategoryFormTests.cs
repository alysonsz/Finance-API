using Bunit;
using Finance.Contracts.Requests.Categories;
using Finance.Web.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;

namespace Finance.Web.Tests.Shared
{
    public class CategoryFormTests : BunitTestHelper
    {
        [Fact]
        public void CategoryForm_Should_RenderCorrectly_WithInitialValues()
        {
            var model = new UpdateCategoryRequest { Title = "Categoria Teste" };

            var cut = RenderComponent<TestLayout>(parameters => parameters
                .AddChildContent<CategoryForm>(childParams => childParams
                    .Add(p => p.Model, model)
                    .Add(p => p.ButtonText, "Salvar Teste")
                )
            );

            var textField = cut.FindComponent<MudBlazor.MudTextField<string>>();
            textField.Should().NotBeNull();
            textField.Instance.Value.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task Submit_Should_InvokeOnValidSubmit_WhenFormIsValid()
        {
            var wasSubmitted = false;
            var model = new UpdateCategoryRequest
            {
                Title = "Um título válido",
                Description = "Descrição válida"
            };

            var cut = RenderComponent<TestLayout>(parameters => parameters
                .AddChildContent<CategoryForm>(childParams => childParams
                    .Add(p => p.Model, model)
                    .Add(p => p.OnValidSubmit, () => wasSubmitted = true)
                )
            );

            await cut.Find("button.mud-button-filled-primary").ClickAsync(new MouseEventArgs());

            wasSubmitted.Should().BeTrue();
        }
    }
}
