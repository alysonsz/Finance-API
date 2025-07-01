using Finance.Contracts.Interfaces.Handlers;
using Microsoft.JSInterop;
using System.Globalization;

namespace Finance.Web.Handlers;

public class AppPreferencesHandler(IJSRuntime js) : IAppPreferencesHandler
{
    private readonly IJSRuntime _js = js;

    public async Task<bool> GetDarkModeAsync()
    {
        var value = await _js.InvokeAsync<string>("localStorage.getItem", "darkMode");
        return bool.TryParse(value, out var result) ? result : false;
    }

    public async Task SetDarkModeAsync(bool enabled)
        => await _js.InvokeVoidAsync("localStorage.setItem", "darkMode", enabled.ToString());

    public async Task<string> GetCurrencyAsync()
    {
        var value = await _js.InvokeAsync<string>("localStorage.getItem", "currency");
        return string.IsNullOrEmpty(value) ? "BRL" : value;
    }

    public async Task SetCurrencyAsync(string currency)
        => await _js.InvokeVoidAsync("localStorage.setItem", "currency", currency);

    public async Task<bool> GetNotifyEmailAsync()
    {
        var value = await _js.InvokeAsync<string>("localStorage.getItem", "notifyEmail");
        return bool.TryParse(value, out var result) ? result : true;
    }

    public async Task SetNotifyEmailAsync(bool enabled)
        => await _js.InvokeVoidAsync("localStorage.setItem", "notifyEmail", enabled.ToString());

    public async Task<bool> GetNotifyPushAsync()
    {
        var value = await _js.InvokeAsync<string>("localStorage.getItem", "notifyPush");
        return bool.TryParse(value, out var result) ? result : false;
    }

    public async Task SetNotifyPushAsync(bool enabled)
        => await _js.InvokeVoidAsync("localStorage.setItem", "notifyPush", enabled.ToString());

    public async Task<CultureInfo> GetCultureAsync()
    {
        var currency = await GetCurrencyAsync();
        return currency switch
        {
            "USD" => new CultureInfo("en-US"),
            "EUR" => new CultureInfo("de-DE"),
            "BRL" or _ => new CultureInfo("pt-BR"),
        };
    }
}
