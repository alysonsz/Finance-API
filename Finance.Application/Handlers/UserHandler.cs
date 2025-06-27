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

    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Response<string>.Fail("Credenciais inválidas.");
        }

        var token = GenerateJwtToken(user);
        return Response<string>.Success(token);
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var issuer = _configuration["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = issuer,
            Audience = issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
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
}
