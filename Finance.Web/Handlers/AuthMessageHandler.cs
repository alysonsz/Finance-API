using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;

namespace Finance.Web.Handlers;

public class AuthMessageHandler(IJSRuntime jsRuntime, IServiceProvider serviceProvider) : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
            navigationManager.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}
