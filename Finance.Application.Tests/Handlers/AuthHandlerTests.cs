using Finance.Application.Features.Auth.GetProfile;
using Finance.Application.Features.Auth.Login;
using Finance.Application.Features.Auth.RefreshToken;
using Finance.Application.Features.Auth.Register;
using Finance.Application.Features.Auth.UpdateProfile;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Finance.Application.Tests.Handlers;

public class AuthHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

    [Fact]
    public async Task Login_Should_ReturnTokens_When_CredentialsAreValid()
    {
        var handler = new LoginUserHandler(_userRepoMock.Object, _tokenMock.Object);

        var command = new LoginUserCommand { Email = "test@email.com", Password = "Password123" };

        var user = new User
        {
            Id = 1,
            Email = command.Email,
            Name = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password)
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _tokenMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("fake-jwt");
        _tokenMock.Setup(t => t.GenerateRefreshToken()).Returns("fake-refresh");

        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("fake-jwt");
        result.Data.RefreshToken.Should().Be("fake-refresh");
        result.Message.Should().Be("Login bem-sucedido!");
    }

    [Fact]
    public async Task Register_Should_ReturnMessage_When_Successful()
    {
        var handler = new RegisterUserHandler(_userRepoMock.Object);

        var command = new RegisterUserCommand
        {
            Name = "Novo Usuário",
            Email = "novo@email.com",
            Password = "Password@123"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().Contain("Usuário registrado");

        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == command.Email &&
            u.Name == command.Name &&
            !string.IsNullOrWhiteSpace(u.PasswordHash))), Times.Once);
    }

    [Fact]
    public async Task Register_Should_Fail_When_EmailAlreadyExists()
    {
        var handler = new RegisterUserHandler(_userRepoMock.Object);

        var command = new RegisterUserCommand
        {
            Email = "existente@email.com",
            Password = "123",
            Name = "X"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(new User { Id = 10, Email = command.Email });

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("O email informado já está em uso.");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetProfile_Should_ReturnProfile_When_UserExists()
    {
        var userId = 123;

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var handler = new GetProfileHandler(_userRepoMock.Object, _httpContextAccessorMock.Object);

        _userRepoMock.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, Name = "User", Email = "u@email.com" });

        var result = await handler.Handle(new GetProfileCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateProfile_Should_ReturnSuccess_When_UserExists()
    {
        var userId = 123;

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var handler = new UpdateProfileHandler(_userRepoMock.Object, _httpContextAccessorMock.Object);

        var existing = new User { Id = userId, Name = "Old", Email = "u@email.com" };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existing);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await handler.Handle(new UpdateProfileCommand { Name = "New Name" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Perfil atualizado com sucesso!");
        result.Data!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task RefreshToken_Should_Fail_When_PrincipalIsInvalid()
    {
        var handler = new RefreshUserTokenHandler(_userRepoMock.Object, _tokenMock.Object);

        _tokenMock.Setup(t => t.GetPrincipalFromExpiredToken(It.IsAny<string>()))
            .Returns((ClaimsPrincipal?)null);

        var result = await handler.Handle(new RefreshUserTokenCommand
        {
            AccessToken = "expired",
            RefreshToken = "refresh"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Token inválido ou expirado.");
    }
}
