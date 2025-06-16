using Finance.Domain.Interfaces.Handlers;
using Finance.Web;
using Finance.Web.Handlers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddHttpClient(
    WebConfiguration.HttpClientName,
    opt =>
    {
        opt.BaseAddress = new Uri("https://localhost:7295");
    });

builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();

await builder.Build().RunAsync();