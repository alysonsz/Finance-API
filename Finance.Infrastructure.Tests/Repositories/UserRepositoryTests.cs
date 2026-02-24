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
    private readonly DbContextOptions<FinanceWriteDbContext> _writeOptions;
    private readonly DbContextOptions<FinanceReadDbContext> _readOptions;

    public UserRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _writeOptions = new DbContextOptionsBuilder<FinanceWriteDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var writeContext = new FinanceWriteDbContext(_writeOptions);
        writeContext.Database.EnsureCreated();

        _readOptions = new DbContextOptionsBuilder<FinanceReadDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var readContext = new FinanceReadDbContext(_readOptions);
        readContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task AddAsync_Should_PersistUser_WhenCalled()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var repository = new UserRepository(readContext, writeContext);
        var newUser = new User 
        { 
            Name = "Herbert", 
            Email = "herbert@email.com", 
            PasswordHash = "some_hash" 
        };

        await repository.AddAsync(newUser);

        newUser.Id.Should().NotBe(0);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var userInDb = await assertContext.Users.FindAsync(newUser.Id);

        userInDb.Should().NotBeNull();
        userInDb!.Name.Should().Be("Herbert");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnUser_WhenExists()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Name = "Test User",
            Email = "test@email.com", 
            PasswordHash = "hash" 
        };

        writeContext.Users.Add(user);

        await writeContext.SaveChangesAsync();

        var repository = new UserRepository(readContext, writeContext);

        var result = await repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("test@email.com");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_BeCaseInsensitive()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var originalEmail = "Case.Test@Email.COM";
        var user = new User 
        { 
            Name = "Case Test", 
            Email = originalEmail, 
            PasswordHash = "hash" 
        };

        writeContext.Users.Add(user);
        await writeContext.SaveChangesAsync();

        var repository = new UserRepository(readContext, writeContext);

        var result = await repository.GetByEmailAsync(originalEmail.ToLower());

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnNull_WhenEmailDoesNotExist()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var repository = new UserRepository(readContext, writeContext);

        var result = await repository.GetByEmailAsync("nonexistent@email.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeDataInDatabase()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var originalUser = new User 
        { 
            Name = "Original Name", 
            Email = "update@test.com", 
            PasswordHash = "hash" 
        };

        writeContext.Users.Add(originalUser);
        await writeContext.SaveChangesAsync();

        writeContext.Entry(originalUser).State = EntityState.Detached;

        var repository = new UserRepository(readContext, writeContext);
        originalUser.Name = "Updated Name";

        await repository.UpdateAsync(originalUser);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var updatedUser = await assertContext.Users.FindAsync(originalUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be("Updated Name");
    }
}