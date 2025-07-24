using Finance.Domain.Models;
using System.Security.Claims;

namespace Finance.Contracts.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
