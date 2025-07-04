using Bunit;
using MudBlazor;
using MudBlazor.Services;

namespace Finance.Web.Tests;

public abstract class BunitTestHelper : TestContext
{
    public BunitTestHelper()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}