using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Tests.Repositories;

public class TransactionRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<FinanceDbContext> _options;

    public TransactionRepositoryTests()
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

    private async Task<User> SeedUserAsync(DbContext context)
    {
        var user = new User { Name = "Test User", Email = "test@email.com", PasswordHash = "123" };
        context.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Category> SeedCategoryAsync(DbContext context, long userId)
    {
        var category = new Category { Title = "Alimentação", UserId = userId, Description = "Test" };
        context.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task CreateAsync_Should_PersistTransaction()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);
        var repository = new TransactionRepository(context);
        var newTransaction = new Transaction { Title = "Almoço", Amount = 50, Type = ETransactionType.Withdraw, UserId = user.Id, CategoryId = category.Id, PaidOrReceivedAt = DateTime.UtcNow };

        var createdTransaction = await repository.CreateAsync(newTransaction);

        createdTransaction.Should().NotBeNull();
        createdTransaction.Id.Should().NotBe(0);

        await using var assertContext = new FinanceDbContext(_options);
        var transactionInDb = await assertContext.Transactions.FindAsync(createdTransaction.Id);
        transactionInDb.Should().NotBeNull();
        transactionInDb!.Amount.Should().Be(50);
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeDataInDatabase()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);
        var originalTransaction = new Transaction { Title = "Original", Amount = 100, PaidOrReceivedAt = DateTime.UtcNow, UserId = user.Id, CategoryId = category.Id, Type = ETransactionType.Withdraw };
        context.Add(originalTransaction);
        await context.SaveChangesAsync();

        context.Entry(originalTransaction).State = EntityState.Detached;

        var repository = new TransactionRepository(context);
        originalTransaction.Amount = 150;

        await repository.UpdateAsync(originalTransaction);

        await using var assertContext = new FinanceDbContext(_options);
        var updatedTransaction = await assertContext.Transactions.FindAsync(originalTransaction.Id);

        updatedTransaction.Should().NotBeNull();
        updatedTransaction!.Amount.Should().Be(150);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnTransactionWithCategory_WhenExists()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);
        var transaction = new Transaction { Title = "Jantar", Amount = 120, Type = ETransactionType.Withdraw, UserId = user.Id, CategoryId = category.Id, PaidOrReceivedAt = DateTime.UtcNow };
        context.Add(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);

        var result = await repository.GetByIdAsync(transaction.Id, user.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Jantar");
        result.Category.Should().NotBeNull();
        result.Category.Title.Should().Be("Alimentação");
    }

    [Fact]
    public async Task GetByPeriodAsync_Should_ReturnCorrectTransactions_ForSpecificDateRange()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);

        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var tomorrow = today.AddDays(1);

        context.Transactions.AddRange(
            new Transaction { Title = "Ontem", Amount = 10, PaidOrReceivedAt = yesterday, UserId = user.Id, CategoryId = category.Id, Type = ETransactionType.Withdraw },
            new Transaction { Title = "Hoje", Amount = 20, PaidOrReceivedAt = today, UserId = user.Id, CategoryId = category.Id, Type = ETransactionType.Withdraw },
            new Transaction { Title = "Amanhã", Amount = 30, PaidOrReceivedAt = tomorrow, UserId = user.Id, CategoryId = category.Id, Type = ETransactionType.Withdraw }
        );
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);

        var result = await repository.GetByPeriodAsync(user.Id, today, today.AddHours(23), 1, 10);

        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result!.First().Title.Should().Be("Hoje");
    }

    [Fact]
    public async Task GetByPeriodAsync_Should_ReturnAllTransactions_WhenDatesAreNull()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);
        context.Transactions.Add(new Transaction { Title = "Qualquer", Amount = 10, PaidOrReceivedAt = DateTime.UtcNow, UserId = user.Id, CategoryId = category.Id, Type = ETransactionType.Withdraw });
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);

        var result = await repository.GetByPeriodAsync(user.Id, null, null, 1, 10);

        result.Should().NotBeNull();
        result.Should().HaveCount(1); 
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveTransactionFromDatabase()
    {
        await using var context = new FinanceDbContext(_options);
        var user = await SeedUserAsync(context);
        var category = await SeedCategoryAsync(context, user.Id);
        var transaction = new Transaction { Title = "Para Deletar", Amount = 99, Type = ETransactionType.Withdraw, UserId = user.Id, CategoryId = category.Id, PaidOrReceivedAt = DateTime.UtcNow };
        context.Add(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context);

        await repository.DeleteAsync(transaction);

        await using var assertContext = new FinanceDbContext(_options);
        var deletedTransaction = await assertContext.Transactions.FindAsync(transaction.Id);
        deletedTransaction.Should().BeNull();
    }
}