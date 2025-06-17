using Finance.Application.Interfaces.Handlers;
using Finance.Application.Requests.Categories;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Finance.Web.Pages.Categories;

public partial class GetAll : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; } = false;
    public List<Category> Categories { get; set; } = [];
    #endregion

    #region Services
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService Dialog { get; set; } = null!;
    [Inject] public ICategoryHandler Handler { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetAllCategoriesRequest();
            var result = await Handler.GetAllAsync(request);
            if (result.IsSuccess && result.Data is not null)
                Categories = result.Data;
            else
                Snackbar.Add(result.Message ?? "Ocorreu um erro ao buscar categorias", Severity.Error);
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
    #endregion

    public async Task OnDeleteButtonClickedAsync(long id, string title)
    {
        var result = await Dialog.ShowMessageBox(
            "ATENÇÃO",
            $"Ao prosseguir, a categoria '{title}' será removida. Deseja continuar?",
            yesText: "Excluir",
            cancelText: "Cancelar");

        if (result is true)
        {
            await OnDeleteAsync(id, title);
            StateHasChanged();
        }
    }

    private async Task OnDeleteAsync(long id, string title)
    {
        try
        {
            var request = new DeleteCategoryRequest { Id = id };
            var result = await Handler.DeleteAsync(request);

            if (result.IsSuccess)
            {
                Categories.RemoveAll(x => x.Id == id);
                Snackbar.Add($"Categoria '{title}' removida com sucesso!", Severity.Info);
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}