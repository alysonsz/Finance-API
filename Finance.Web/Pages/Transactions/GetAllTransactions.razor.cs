using Finance.Application.Interfaces.Handlers;
using Finance.Domain.Common;
using Finance.Domain.Models;
using Finance.Domain.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Finance.Web.Pages.Transactions;

public partial class GetAll : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; } = false;
    public List<Transaction> Transactions { get; set; } = [];
    public DateTime? StartDate { get; set; } = DateTime.Now.GetFirstDay();
    public DateTime? EndDate { get; set; } = DateTime.Now.GetLastDay();
    #endregion

    #region Services
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService Dialog { get; set; } = null!;
    [Inject] public ITransactionHandler Handler { get; set; } = null!;
    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        await GetTransactionsAsync();
    }
    #endregion

    public async Task GetTransactionsAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetTransactionsByPeriodRequest
            {
                StartDate = StartDate,
                EndDate = EndDate
            };

            var result = await Handler.GetByPeriodAsync(request);
            if (result.IsSuccess && result.Data is not null)
                Transactions = result.Data;
            else
                Transactions = new List<Transaction>();

            if (!result.IsSuccess)
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}