using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Categories;
using Finance.Domain.Models.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace Finance.Web.Pages.Categories;

public partial class GetAll : ComponentBase
{
    #region Properties
    public bool IsBusy { get; set; } = false;
    public List<CategoryDto> Categories { get; set; } = [];
    #endregion

    #region Services
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService Dialog { get; set; } = null!;
    [Inject] public ICategoryHandler Handler { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }
    #endregion

    private static long? GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst("sub") ?? user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("nameid");

        if (claim != null && long.TryParse(claim.Value, out var id))
            return id;
        return null;
    }

    private async Task LoadCategoriesAsync()
    {
        IsBusy = true;
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                Snackbar.Add("Usuário não autenticado.", Severity.Error);
                IsBusy = false;
                return;
            }

            var userId = GetUserId(user);
            if (userId == null)
            {
                Snackbar.Add("UserId não encontrado nas claims.", Severity.Error);
                IsBusy = false;
                return;
            }

            var request = new GetAllCategoriesRequest
            {
                UserId = userId.Value,
                PageNumber = 1,
                PageSize = 25
            };

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