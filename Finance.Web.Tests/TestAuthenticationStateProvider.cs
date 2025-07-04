using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _user = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_user));

    public void SetAuthenticatedUser(string userId)
    {
        _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", userId)
        }, "test"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetCustomUser(IEnumerable<Claim> claims)
    {
        _user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetAnonymousUser()
    {
        _user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
