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
    private readonly DbContextOptions<FinanceWriteDbContext> _writeOptions;
    private readonly DbContextOptions<FinanceReadDbContext> _readOptions;

    public TransactionRepositoryTests()
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

    private async Task<User> SeedUserAsync(DbContext context)
    {
        var userResult = User.Create("Test User", "test@email.com", "123");
        var user = userResult.Value;

        context.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Category> SeedCategoryAsync(DbContext context, long userId)
    {
        var catResult = Category.Create("Alimentação", "Test", userId);
        var category = catResult.Value;

        context.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task CreateAsync_Should_PersistTransaction()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);
        var repository = new TransactionRepository(readContext, writeContext);
        var txResult = Transaction.Create("Almoço", 50, ETransactionType.Withdraw, user.Id, category.Id, DateTime.UtcNow);
        var newTransaction = txResult.Value;

        var createdTransaction = await repository.CreateAsync(newTransaction);

        createdTransaction.Should().NotBeNull();
        createdTransaction.Id.Should().NotBe(0);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var transactionInDb = await assertContext.Transactions.FindAsync(createdTransaction.Id);
        transactionInDb.Should().NotBeNull();
        transactionInDb!.Amount.Should().Be(-50);
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeDataInDatabase()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);
        var txResult = Transaction.Create("Original", 100, ETransactionType.Withdraw, user.Id, category.Id, DateTime.UtcNow);
        var originalTransaction = txResult.Value;

        writeContext.Add(originalTransaction);
        await writeContext.SaveChangesAsync();

        writeContext.Entry(originalTransaction).State = EntityState.Detached;

        var repository = new TransactionRepository(readContext, writeContext);
        typeof(Transaction).GetProperty("Amount")?.SetValue(originalTransaction, 150m);

        await repository.UpdateAsync(originalTransaction);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var updatedTransaction = await assertContext.Transactions.FindAsync(originalTransaction.Id);

        updatedTransaction.Should().NotBeNull();
        updatedTransaction!.Amount.Should().Be(150);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnTransactionWithCategory_WhenExists()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);
        var txResult = Transaction.Create("Jantar", 120, ETransactionType.Withdraw, user.Id, category.Id, DateTime.UtcNow);
        var transaction = txResult.Value;

        writeContext.Add(transaction);
        await writeContext.SaveChangesAsync();

        var repository = new TransactionRepository(readContext, writeContext);

        var result = await repository.GetByIdAsync(transaction.Id, user.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Jantar");
        result.Category.Should().NotBeNull();
        result.Category.Title.Should().Be("Alimentação");
    }

    [Fact]
    public async Task GetByPeriodAsync_Should_ReturnCorrectTransactions_ForSpecificDateRange()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);

        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var tomorrow = today.AddDays(1);

        writeContext.Transactions.AddRange(
            Transaction.Create("Ontem", 10, ETransactionType.Withdraw, user.Id, category.Id, yesterday).Value,
            Transaction.Create("Hoje", 20, ETransactionType.Withdraw, user.Id, category.Id, today).Value,
            Transaction.Create("Amanhã", 30, ETransactionType.Withdraw, user.Id, category.Id, tomorrow).Value
        );
        await writeContext.SaveChangesAsync();

        var repository = new TransactionRepository(readContext, writeContext);

        var result = await repository.GetByPeriodAsync(user.Id, today, today.AddHours(23), 1, 10);

        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result!.First().Title.Should().Be("Hoje");
    }

    [Fact]
    public async Task GetByPeriodAsync_Should_ReturnAllTransactions_WhenDatesAreNull()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);
        var txResult = Transaction.Create("Qualquer", 10, ETransactionType.Withdraw, user.Id, category.Id, DateTime.UtcNow);
        writeContext.Transactions.Add(txResult.Value);

        await writeContext.SaveChangesAsync();

        var repository = new TransactionRepository(readContext, writeContext);

        var result = await repository.GetByPeriodAsync(user.Id, null, null, 1, 10);

        result.Should().NotBeNull();
        result.Should().HaveCount(1); 
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveTransactionFromDatabase()
    {
        await using var writeContext = new FinanceWriteDbContext(_writeOptions);
        await using var readContext = new FinanceReadDbContext(_readOptions);

        var user = await SeedUserAsync(writeContext);
        var category = await SeedCategoryAsync(writeContext, user.Id);
        var txResult = Transaction.Create("Para Deletar", 99, ETransactionType.Withdraw, user.Id, category.Id, DateTime.UtcNow);
        var transaction = txResult.Value;

        writeContext.Add(transaction);
        await writeContext.SaveChangesAsync();

        var repository = new TransactionRepository(readContext, writeContext);

        await repository.DeleteAsync(transaction);

        await using var assertContext = new FinanceReadDbContext(_readOptions);
        var deletedTransaction = await assertContext.Transactions.FindAsync(transaction.Id);
        deletedTransaction.Should().BeNull();
    }
}