using System.Globalization;

namespace Finance.Contracts.Interfaces.Handlers;

public interface IAppPreferencesHandler
{
    Task<bool> GetDarkModeAsync();
    Task SetDarkModeAsync(bool enabled);
    Task<string> GetCurrencyAsync();
    Task SetCurrencyAsync(string currency);
    Task<bool> GetNotifyEmailAsync();
    Task SetNotifyEmailAsync(bool enabled);
    Task<bool> GetNotifyPushAsync();
    Task SetNotifyPushAsync(bool enabled);
    Task<CultureInfo> GetCultureAsync();
}
