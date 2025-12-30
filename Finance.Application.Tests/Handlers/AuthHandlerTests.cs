using Finance.Application.Commands.Users;
using Finance.Application.Handlers.Users;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Responses.Auth;
using Finance.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace Finance.Application.Tests.Handlers;

public class AuthHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ITokenService> _mockToken;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly GetProfileHandler _getProfileHandler;
    private readonly LoginUserHandler _loginUserHandler;
    private readonly RefreshUserTokenHandler _refreshTokenHandler;
    private readonly RegisterUserHandler _registerUserHandler;
    private readonly UpdateProfileHandler _updateProfileHandler;

    public AuthHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockToken = new Mock<ITokenService>();
        _mockConfig = new Mock<IConfiguration>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _getProfileHandler = new GetProfileHandler(_mockUserRepo.Object, _mockHttpContextAccessor.Object);
        _loginUserHandler = new LoginUserHandler(_mockUserRepo.Object, _mockToken.Object);
        _refreshTokenHandler = new RefreshUserTokenHandler(_mockUserRepo.Object, _mockToken.Object);
        _registerUserHandler = new RegisterUserHandler(_mockUserRepo.Object, _mockToken.Object);
        _updateProfileHandler = new UpdateProfileHandler(_mockUserRepo.Object, _mockHttpContextAccessor.Object);

        var fakeJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.fakePayload.fakeSignature";

        _mockToken.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns(fakeJwt);
        _mockToken.Setup(t => t.GenerateRefreshToken()).Returns("fake-refresh-token");

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

        _getProfileHandler = new GetProfileHandler(
            _mockUserRepo.Object,
            _mockHttpContextAccessor.Object
        );
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnToken_When_CredentialsAreValid()
    {
        var request = new LoginUserCommand { Email = "teste@email.com", Password = "Password123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var existingUser = new User
        {
            Id = 1,
            Email = request.Email,
            Name = "Usuário Teste",
            PasswordHash = hashedPassword
        };

        var user = new LoginResponse { AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _loginUserHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.AccessToken.Should().Contain("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");

        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnFail_When_UserNotFound()
    {
        var request = new LoginUserCommand { Email = "naoexiste@email.com", Password = "Password123" };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        var result = await _loginUserHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnFail_When_PasswordIsIncorrect()
    {
        var request = new LoginUserCommand { Email = "teste@email.com", Password = "wrong_password" };
        var existingUser = new User
        {
            Id = 1,
            Email = request.Email,
            Name = "Usuário Teste",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _loginUserHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task RegisterAsync_Should_ReturnToken_When_RegistrationIsSuccessful()
    {
        var request = new RegisterUserCommand
        {
            Name = "Novo Usuário",
            Email = "novo@email.com",
            Password = "Password@123"
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _registerUserHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.AccessToken.Should().Contain("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");

        result.Data.RefreshToken.Should().NotBeNullOrEmpty();

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
        var request = new RegisterUserCommand { Email = "existente@email.com", Password = "123" };
        var existingUser = new User { Id = 99, Email = request.Email };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var result = await _registerUserHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("O email informado já está em uso.");

        _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetProfileAsync_Should_ReturnUserProfile_When_UserIsFound()
    {
        var userId = 123;
        var existingUser = new User { Id = userId, Name = "Usuário Teste", Email = "teste@email.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var command = new GetProfileCommand();

        new UserProfileResponse { Id = userId, Name = "Usuário Teste", Email = "teste@email.com" };

        var result = await _getProfileHandler.Handle(command, CancellationToken.None);

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

        var command = new GetProfileCommand();

        new UserProfileResponse { Id = userId, Name = "Usuário Teste", Email = "teste@email.com" };

        var result = await _getProfileHandler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Usuário não encontrado.");
    }

    [Fact]
    public async Task UpdateProfileAsync_Should_ReturnSuccess_When_UserIsFoundAndUpdated()
    {
        var request = new UpdateProfileCommand { Name = "Nome Atualizado" };
        var userId = 123;
        var existingUser = new User { Id = userId, Name = "Nome Antigo", Email = "teste@email.com" };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _updateProfileHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data?.Name.Should().Be(request.Name);
        result.Message.Should().Be("Perfil atualizado com sucesso!");

        _mockUserRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Name == request.Name)), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_Should_ReturnFail_When_UserIsNotFound()
    {
        var request = new UpdateProfileCommand { Name = "Nome Atualizado" };
        var userId = 123;

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var result = await _updateProfileHandler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Usuário não encontrado.");

        _mockUserRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}