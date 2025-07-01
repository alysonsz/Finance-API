namespace Finance.Web.Handlers;

public class AppThemeHandler
{
    public event Func<bool, Task>? OnThemeChanged;
    public bool IsDarkMode { get; private set; }

    public async Task ApplyTheme(bool isDark)
    {
        IsDarkMode = isDark;
        if (OnThemeChanged is not null)
            await OnThemeChanged.Invoke(isDark);
    }

    public async Task ToggleTheme()
    {
        await ApplyTheme(!IsDarkMode);
    }
}
