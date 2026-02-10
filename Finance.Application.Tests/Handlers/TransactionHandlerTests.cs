using Finance.Application.Features.Transactions.Create;
using Finance.Application.Features.Transactions.Delete;
using Finance.Application.Features.Transactions.GetById;
using Finance.Application.Features.Transactions.GetByPeriod;
using Finance.Application.Features.Transactions.GetReport;
using Finance.Application.Features.Transactions.Update;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using FluentAssertions;
using Moq;

namespace Finance.Application.Tests.Handlers;

public class TransactionHandlerTests
{
    private readonly Mock<ITransactionRepository> _txRepoMock = new();
    private readonly Mock<ICategoryRepository> _catRepoMock = new();

    [Fact]
    public async Task Create_Should_Return201_When_CategoryExists()
    {
        var handler = new CreateTransactionHandler(_txRepoMock.Object, _catRepoMock.Object);

        var command = new CreateTransactionCommand
        {
            UserId = 123,
            Title = "Salário",
            Amount = 5000,
            Type = ETransactionType.Deposit,
            CategoryId = 1,
            PaidOrReceivedAt = DateTime.UtcNow
        };

        _catRepoMock.Setup(r => r.GetByIdAsync(command.CategoryId, command.UserId))
            .ReturnsAsync(new Category { Id = 1, UserId = 123, Title = "Trabalho" });

        _txRepoMock.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => t);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(201);
        result.Message.Should().Be("Transação criada com sucesso!");
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);

        _txRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task Create_Should_Return404_When_CategoryNotFound()
    {
        var handler = new CreateTransactionHandler(_txRepoMock.Object, _catRepoMock.Object);

        var command = new CreateTransactionCommand
        {
            UserId = 123,
            CategoryId = 99,
            Amount = 10,
            Type = ETransactionType.Deposit,
            Title = "X"
        };

        _catRepoMock.Setup(r => r.GetByIdAsync(command.CategoryId, command.UserId))
            .ReturnsAsync((Category?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(404);
        result.Message.Should().Be("Categoria não encontrada.");
        result.Data.Should().BeNull();

        _txRepoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task Update_Should_Return404_When_TransactionNotFound()
    {
        var handler = new UpdateTransactionHandler(_txRepoMock.Object, _catRepoMock.Object);

        var command = new UpdateTransactionCommand 
        {
            Id = 99,
            UserId = 123,
            CategoryId = 1,
            Title = "X",
            Amount = 10,
            Type = ETransactionType.Deposit,
            PaidOrReceivedAt = DateTime.UtcNow
        };

        _txRepoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync((Transaction?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(404);
        result.Message.Should().Be("Transação não encontrada.");
        _catRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        _txRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Return200_When_Found()
    {
        var handler = new DeleteTransactionHandler(_txRepoMock.Object, _catRepoMock.Object);

        var command = new DeleteTransactionCommand { Id = 1, UserId = 123 };

        var existingTx = new Transaction
        {
            Id = 1,
            UserId = 123,
            Title = "A pagar",
            CategoryId = 10,
            Amount = -50,
            Type = ETransactionType.Withdraw,
            PaidOrReceivedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var category = new Category 
        { 
            Id = 10,
            UserId = 123,
            Title = "Casa" 
        };

        _txRepoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync(existingTx);

        _catRepoMock.Setup(r => r.GetByIdAsync(existingTx.CategoryId, command.UserId))
            .ReturnsAsync(category);

        _txRepoMock.Setup(r => r.DeleteAsync(existingTx))
            .ReturnsAsync(existingTx);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(200);
        result.Message.Should().Be("Transação excluída com sucesso!");
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(existingTx.Id);

        _txRepoMock.Verify(r => r.DeleteAsync(existingTx), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_Return404_When_TransactionNotFound()
    {
        var handler = new DeleteTransactionHandler(_txRepoMock.Object, _catRepoMock.Object);

        var command = new DeleteTransactionCommand 
        { 
            Id = 99, 
            UserId = 123
        };

        _txRepoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync((Transaction?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(404);
        result.Message.Should().Be("Transação não encontrada.");
        _txRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public async Task GetByPeriod_Should_ReturnPagedData_When_Successful()
    {
        var handler = new GetByPeriodTransactionHandler(_txRepoMock.Object);

        var command = new GetByPeriodTransactionCommand
        {
            UserId = 123,
            PageNumber = 1,
            PageSize = 2,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        _txRepoMock.Setup(r => r.CountByPeriodAsync(command.UserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(3);

        var txs = new List<Transaction>
        {
            new()
            {
                Id = 1, UserId = 123, Title = "Salário", Amount = 5000, Type = ETransactionType.Deposit,
                PaidOrReceivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow,
                Category = new Category { Id = 1, Title = "Trabalho" }
            },
            new()
            {
                Id = 2, UserId = 123, Title = "Aluguel", Amount = -1500, Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow,
                Category = new Category { Id = 2, Title = "Casa" }
            }
        };

        _txRepoMock.Setup(r => r.GetByPeriodAsync(command.UserId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), command.PageNumber, command.PageSize))
            .ReturnsAsync(txs);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
        result.TotalCount.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task GetReport_Should_ReturnSuccess_When_Successful()
    {
        var handler = new GetReportTransactionHandler(_txRepoMock.Object);

        var command = new GetReportTransactionCommand
        {
            UserId = 123,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };

        var txs = new List<Transaction>
        {
            new() { Amount = -100, Category = new Category { Title = "Casa" } },
            new() { Amount = -50, Category = new Category { Title = "Casa" } },
            new() { Amount = 5000, Category = new Category { Title = "Trabalho" } }
        };

        _txRepoMock.Setup(r => r.GetAllByPeriodAsync(command.UserId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(txs);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Relatório gerado com sucesso.");
        result.Data!.TotalExpenses.Should().Be(150);
        result.Data.TotalIncomes.Should().Be(5000);
    }
}
