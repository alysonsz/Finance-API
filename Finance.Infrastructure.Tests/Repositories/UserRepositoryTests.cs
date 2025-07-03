using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<FinanceDbContext> _options;

    public UserRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new FinanceDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task AddAsync_Should_PersistUser_WhenCalled()
    {
        await using var context = new FinanceDbContext(_options);
        var repository = new UserRepository(context);
        var newUser = new User { Name = "Herbert", Email = "herbert@email.com", PasswordHash = "some_hash" };

        await repository.AddAsync(newUser);

        newUser.Id.Should().NotBe(0);

        await using var assertContext = new FinanceDbContext(_options);
        var userInDb = await assertContext.Users.FindAsync(newUser.Id);

        userInDb.Should().NotBeNull();
        userInDb!.Name.Should().Be("Herbert");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnUser_WhenExists()
    {
        await using var context = new FinanceDbContext(_options);
        var user = new User { Name = "Test User", Email = "test@email.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("test@email.com");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_BeCaseInsensitive()
    {
        await using var context = new FinanceDbContext(_options);
        var originalEmail = "Case.Test@Email.COM";
        var user = new User { Name = "Case Test", Email = originalEmail, PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync(originalEmail.ToLower());

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnNull_WhenEmailDoesNotExist()
    {
        await using var context = new FinanceDbContext(_options);
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("nonexistent@email.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeDataInDatabase()
    {
        await using var context = new FinanceDbContext(_options);
        var originalUser = new User { Name = "Original Name", Email = "update@test.com", PasswordHash = "hash" };
        context.Users.Add(originalUser);
        await context.SaveChangesAsync();

        context.Entry(originalUser).State = EntityState.Detached;

        var repository = new UserRepository(context);
        originalUser.Name = "Updated Name";

        await repository.UpdateAsync(originalUser);

        await using var assertContext = new FinanceDbContext(_options);
        var updatedUser = await assertContext.Users.FindAsync(originalUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("Updated Name");
    }
}