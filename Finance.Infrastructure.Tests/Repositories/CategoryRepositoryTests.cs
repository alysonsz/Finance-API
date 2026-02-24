using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Tests.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<FinanceWriteDbContext> _writeOptions;
    private readonly DbContextOptions<FinanceReadDbContext> _readOptions;

    public CategoryRepositoryTests()
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

    private async Task SeedUserAsync(DbContext context, User user)
    {
        context.Add(user);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAsync_Should_PersistCategory_WhenCalled()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Id = 1, 
            Name = "Test User", 
            Email = "test@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user);

        var repository = new CategoryRepository(readContext, writeContext);
        var newCategory = new Category 
        { 
            Title = "Lazer", 
            Description = "Gastos com Lazer", 
            UserId = user.Id 
        };

        var createdCategory = await repository.CreateAsync(newCategory);

        createdCategory.Should().NotBeNull();
        createdCategory.Id.Should().NotBe(0);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var categoryInDb = await assertContext.Categories.FindAsync(createdCategory.Id);

        categoryInDb.Should().NotBeNull();
        categoryInDb!.Title.Should().Be("Lazer");
        categoryInDb.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnCategory_WhenExistsForUser()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Id = 1, 
            Name = "Test User", 
            Email = "test@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user);

        var category = new Category 
        { 
            Title = "Saúde", 
            Description = "Gastos com Saúde", 
            UserId = user.Id 
        };

        writeContext.Categories.Add(category);
        await writeContext.SaveChangesAsync();

        var repository = new CategoryRepository(readContext, writeContext);

        var result = await repository.GetByIdAsync(category.Id, user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenDoesNotExistForUser()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Id = 1, 
            Name = "Test User",
            Email = "test@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user);

        var repository = new CategoryRepository(readContext, writeContext);

        var result = await repository.GetByIdAsync(999, user.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnOnlyCategoriesForGivenUser()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user1 = new User 
        { 
            Id = 1, 
            Name = "User One", 
            Email = "user1@email.com", 
            PasswordHash = "123" 
        };

        var user2 = new User 
        { 
            Id = 2, 
            Name = "User Two", 
            Email = "user2@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user1);
        await SeedUserAsync(writeContext, user2);

        writeContext.Categories.AddRange(
            new Category { Title = "Moradia", UserId = user1.Id },
            new Category { Title = "Transporte", UserId = user1.Id },
            new Category { Title = "Alimentação", UserId = user2.Id }
        );
        await writeContext.SaveChangesAsync();

        var repository = new CategoryRepository(readContext, writeContext);

        var categoriesUser1 = await repository.GetAllAsync(user1.Id);
        var categoriesUser2 = await repository.GetAllAsync(user2.Id);

        categoriesUser1.Should().NotBeNull();
        categoriesUser1!.Count.Should().Be(2);
        categoriesUser1.Should().OnlyContain(c => c.UserId == user1.Id);

        categoriesUser2.Should().NotBeNull();
        categoriesUser2!.Count.Should().Be(1);
        categoriesUser2.Should().ContainSingle(c => c.Title == "Alimentação");
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveCategoryFromDatabase()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Id = 1, 
            Name = "Test User", 
            Email = "test@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user);

        var category = new Category 
        { 
            Title = "Para Deletar", 
            UserId = user.Id 
        };

        writeContext.Categories.Add(category);
        await writeContext.SaveChangesAsync();

        var repository = new CategoryRepository(readContext, writeContext);

        await repository.DeleteAsync(category);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var deletedCategory = await assertContext.Categories.FindAsync(category.Id);
        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeDataInDatabase()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = new User 
        { 
            Id = 1, 
            Name = "Test User", 
            Email = "test@email.com", 
            PasswordHash = "123" 
        };

        await SeedUserAsync(writeContext, user);

        var originalCategory = new Category
        { 
            Title = "Original", 
            Description = "Original Desc", 
            UserId = user.Id 
        };

        writeContext.Categories.Add(originalCategory);
        await writeContext.SaveChangesAsync();

        writeContext.Entry(originalCategory).State = EntityState.Detached;

        var repository = new CategoryRepository(readContext, writeContext);
        originalCategory.Title = "Atualizado";

        await repository.UpdateAsync(originalCategory);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var updatedCategory = await assertContext.Categories.FindAsync(originalCategory.Id);

        updatedCategory.Should().NotBeNull();
        updatedCategory!.Title.Should().Be("Atualizado");
    }
}