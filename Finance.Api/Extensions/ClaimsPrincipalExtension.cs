using System.Security.Claims;

namespace Finance.API.Extensions;

public static class ClaimsPrincipalExtension
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("sub") ?? user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("nameid");
        return claim != null && long.TryParse(claim.Value, out var id)
            ? id
            : throw new UnauthorizedAccessException("Usuário não autenticado");
    }
}
