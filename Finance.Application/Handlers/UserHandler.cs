using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Requests.Auth;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Auth;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Finance.Application.Handlers;

public class UserHandler(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor contextAccessor) : IUserHandler
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpContextAccessor _context = contextAccessor;

    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Response<string>.Fail("Email já está em uso.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);

        var token = GenerateJwtToken(user);
        return Response<string>.Success(token);
    }

    public async Task<Response<LoginResponse?>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Response<LoginResponse?>.Fail("Credenciais inválidas.");
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        var loginResponse = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Response<LoginResponse?>.Success(loginResponse, "Login bem-sucedido!");
    }

    public async Task<Response<LoginResponse?>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        var userId = long.Parse(principal.Identity.Name);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Response<LoginResponse?>.Fail("Token inválido ou expirado");
        }

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        var loginResponse = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return Response<LoginResponse?>.Success(loginResponse, "Token renovado com sucesso!");
    }

    public async Task<Response<UserProfileResponse?>> GetProfileAsync()
    {
        var userId = _context.HttpContext!.User.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Response<UserProfileResponse?>.Fail("Usuário não encontrado");

        var dto = new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
        return Response<UserProfileResponse?>.Success(dto);
    }

    public async Task<Response<UserProfileResponse?>> UpdateProfileAsync(UpdateUserProfileRequest request)
    {
        var userId = _context.HttpContext!.User.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Response<UserProfileResponse?>.Fail("Usuário não encontrado");

        user.Name = request.Name;
        var updated = await _userRepository.UpdateAsync(user);

        var dto = new UserProfileResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Email = updated.Email
        };
        return Response<UserProfileResponse?>.Success(dto, "Perfil atualizado com sucesso");
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("unique_name", user.Name),
            new Claim("email", user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido");
        }

        return principal;
    }
}
