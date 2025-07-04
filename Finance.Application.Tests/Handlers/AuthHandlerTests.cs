using Finance.Application.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Requests.Auth;
using Finance.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace Finance.Application.Tests.Handlers;

public class UserHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly UserHandler _handler;

    public UserHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockConfig = new Mock<IConfiguration>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        var myConfiguration = new Dictionary<string, string?>
    {
        {"Jwt:Key", "uma_chave_secreta_super_longa_e_valida_para_testes"},
        {"Jwt:Issuer", "FinanceAppIssuer"},
        {"Jwt:Audience", "FinanceAppAudience"}
    };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        var testUserId = "123";
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, testUserId) };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        _handler = new UserHandler(
            _mockUserRepo.Object,
            configuration,
            _mockHttpContextAccessor.Object
        );
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnToken_When_CredentialsAreValid()
    {
        var request = new LoginRequest { Email = "teste@email.com", Password = "Password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var existingUser = new User
        {
            Id = 1,
            Email = request.Email,
            Name = "Usuário Teste",
            PasswordHash = hashedPassword
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _handler.LoginAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Should().Contain("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnFail_When_UserNotFound()
    {
        var request = new LoginRequest { Email = "naoexiste@email.com", Password = "Password123" };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        var result = await _handler.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnFail_When_PasswordIsIncorrect()
    {
        var request = new LoginRequest { Email = "teste@email.com", Password = "wrong_password" };
        var existingUser = new User
        {
            Id = 1,
            Email = request.Email,
            Name = "Usuário Teste",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _handler.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task RegisterAsync_Should_ReturnToken_When_RegistrationIsSuccessful()
    {
        var request = new RegisterRequest
        {
            Name = "Novo Usuário",
            Email = "novo@email.com",
            Password = "Password@123"
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.RegisterAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();

        _mockUserRepo.Verify(r => r.GetByEmailAsync(request.Email), Times.Once);
        _mockUserRepo.Verify(r => r.AddAsync(

            It.Is<User>(u =>
                u.Email == request.Email &&
                u.Name == request.Name &&
                !string.IsNullOrEmpty(u.PasswordHash))),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Should_ReturnFail_When_EmailAlreadyExists()
    {
        var request = new RegisterRequest { Email = "existente@email.com", Password = "123" };
        var existingUser = new User { Id = 99, Email = request.Email };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _handler.RegisterAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Email já está em uso.");

        _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
    [Fact]
    public async Task GetProfileAsync_Should_ReturnUserProfile_When_UserIsFound()
    {
        var userId = 123;
        var existingUser = new User { Id = userId, Name = "Usuário Teste", Email = "teste@email.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var result = await _handler.GetProfileAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data?.Id.Should().Be(userId);
        result.Data?.Name.Should().Be(existingUser.Name);
    }

    [Fact]
    public async Task GetProfileAsync_Should_ReturnFail_When_UserIsNotFoundInRepository()
    {
        var userId = 123;

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var result = await _handler.GetProfileAsync();

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Usuário não encontrado");
    }

    [Fact]
    public async Task UpdateProfileAsync_Should_ReturnSuccess_When_UserIsFoundAndUpdated()
    {
        var request = new UpdateUserProfileRequest { Name = "Nome Atualizado" };
        var userId = 123;
        var existingUser = new User { Id = userId, Name = "Nome Antigo", Email = "teste@email.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _handler.UpdateProfileAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data?.Name.Should().Be(request.Name);
        result.Message.Should().Be("Perfil atualizado com sucesso");

        _mockUserRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Name == request.Name)), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_Should_ReturnFail_When_UserIsNotFound()
    {
        var request = new UpdateUserProfileRequest { Name = "Nome Atualizado" };
        var userId = 123;

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var result = await _handler.UpdateProfileAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Usuário não encontrado");

        _mockUserRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}