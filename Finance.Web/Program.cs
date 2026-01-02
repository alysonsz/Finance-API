using Finance.Contracts.Interfaces.Handlers;
using Finance.Web;
using Finance.Web.Authentication;
using Finance.Web.Handlers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddSingleton<AppThemeHandler>();
builder.Services.AddScoped<IAppPreferencesHandler, AppPreferencesHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient(WebConfiguration.HttpClientName, client =>
{
    client.BaseAddress = new Uri("http://localhost:7279");
}).AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient(WebConfiguration.HttpClientName));

builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
builder.Services.AddTransient<AuthHandler, AuthHandler>();

var host = builder.Build();

await SetDefaultCultureAsync(host.Services);
await host.RunAsync();

static async Task SetDefaultCultureAsync(IServiceProvider services)
{
    await using var scope = services.CreateAsyncScope();
    var preferenceHandler = scope.ServiceProvider.GetRequiredService<IAppPreferencesHandler>();

    var culture = await preferenceHandler.GetCultureAsync();

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}