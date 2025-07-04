using Bunit;
using Finance.Contracts.Requests.Transactions;
using Finance.Domain.Enums;
using Finance.Domain.Models.DTOs;
using Finance.Web.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Finance.Web.Tests.Shared
{
    public class TransactionFormTests : BunitTestHelper
    {
        [Fact]
        public void TransactionForm_Should_RenderCorrectly_WithInitialValues()
        {
            var model = new UpdateTransactionRequest { CategoryId = 1 };
            var categories = new List<CategoryDto> { new() { Id = 1, Title = "Alimentação" } };

            var cut = RenderComponent<TestLayout>(parameters => parameters
                .AddChildContent<TransactionForm>(childParams => childParams
                    .Add(p => p.Model, model)
                    .Add(p => p.Categories, categories)
                )
            );

            cut.FindComponent<MudSelect<long>>().Instance.Value.Should().Be(1);
        }

        [Fact]
        public async Task Submit_Should_InvokeOnValidSubmit_WhenFormIsValid()
        {
            var wasSubmitted = false;
            var model = new UpdateTransactionRequest
            {
                Title = "Salário",
                Amount = 5000m,
                CategoryId = 1,
                PaidOrReceivedAt = DateTime.Now,
                Type = ETransactionType.Deposit
            };
            var categories = new List<CategoryDto> { new() { Id = 1, Title = "Salário" } };

            var cut = RenderComponent<TestLayout>(parameters => parameters
                .AddChildContent<TransactionForm>(childParams => childParams
                    .Add(p => p.Model, model)
                    .Add(p => p.Categories, categories)
                    .Add(p => p.OnValidSubmit, () => wasSubmitted = true)
                )
            );

            var submitButton = cut.Find("button.mud-button-filled-primary");
            await submitButton.ClickAsync(new MouseEventArgs());

            wasSubmitted.Should().BeTrue();
        }
    }
}
