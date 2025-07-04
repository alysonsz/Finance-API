using Finance.Application.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Requests.Transactions;
using Finance.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Finance.Application.Tests.Handlers;

public class TransactionHandlerTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly TransactionHandler _handler;

    public TransactionHandlerTests()
    {
        _mockTransactionRepo = new Mock<ITransactionRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();

        _handler = new TransactionHandler(
            _mockTransactionRepo.Object,
            _mockCategoryRepo.Object
        );
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnCreated_When_CategoryExistsAndTransactionIsCreated()
    {
        var request = new CreateTransactionRequest
        {
            Title = "Salário",
            Amount = 5000,
            Type = ETransactionType.Deposit,
            CategoryId = 1,
            UserId = 123
        };

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(request.CategoryId, request.UserId))
            .ReturnsAsync(new Domain.Models.Category { Id = request.CategoryId, UserId = request.UserId });

        _mockTransactionRepo.Setup(r => r.CreateAsync(It.IsAny<Domain.Models.Transaction>()))
            .ReturnsAsync((Domain.Models.Transaction t) => t);

        var result = await _handler.CreateAsync(request);

        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Transação criada com sucesso!");
        result.Data?.Title.Should().Be(request.Title);

        _mockCategoryRepo.Verify(r => r.GetByIdAsync(request.CategoryId, request.UserId), Times.Once);
        _mockTransactionRepo.Verify(r => r.CreateAsync(It.IsAny<Domain.Models.Transaction>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_When_CategoryDoesNotExist()
    {
        var request = new CreateTransactionRequest
        {
            CategoryId = 99,
            UserId = 123
        };

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(request.CategoryId, request.UserId))
            .ReturnsAsync((Domain.Models.Category?)null);

        var result = await _handler.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Categoria não encontrada");
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnSuccess_When_TransactionAndCategoryExist()
    {
        var request = new UpdateTransactionRequest
        {
            Id = 1,
            CategoryId = 2,
            Title = "Cinema",
            Amount = 75,
            UserId = 123
        };

        var existingTransaction = new Domain.Models.Transaction { Id = 1, UserId = 123, Title = "Jantar" };
        var newCategory = new Domain.Models.Category { Id = 2, UserId = 123, Title = "Lazer" };

        _mockTransactionRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingTransaction);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(request.CategoryId, request.UserId))
            .ReturnsAsync(newCategory);

        _mockTransactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Transaction>()))
            .ReturnsAsync((Domain.Models.Transaction t) => t);

        var result = await _handler.UpdateAsync(request);

        result.Data.Should().NotBeNull();
        result.Data?.Title.Should().Be(request.Title);
        result.Data?.Amount.Should().Be(request.Amount);
        result.Message.Should().Be("Transação atualizada com sucesso!");

        _mockTransactionRepo.Verify(r => r.GetByIdAsync(request.Id, request.UserId), Times.Once);
        _mockCategoryRepo.Verify(r => r.GetByIdAsync(request.CategoryId, request.UserId), Times.Once);
        _mockTransactionRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Transaction>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnNotFound_When_TransactionDoesNotExist()
    {
        var request = new UpdateTransactionRequest { Id = 99, UserId = 123 };

        _mockTransactionRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync((Domain.Models.Transaction?)null);

        var result = await _handler.UpdateAsync(request);

        result.Data.Should().BeNull();
        result.Message.Should().Be("Transação não encontrada");

        _mockCategoryRepo.Verify(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
        _mockTransactionRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Transaction>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnBadRequest_When_NewCategoryDoesNotExist()
    {
        var request = new UpdateTransactionRequest { Id = 1, CategoryId = 99, UserId = 123 };
        var existingTransaction = new Domain.Models.Transaction { Id = 1, UserId = 123 };

        _mockTransactionRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingTransaction);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(request.CategoryId, request.UserId))
            .ReturnsAsync((Domain.Models.Category?)null);

        var result = await _handler.UpdateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Categoria vinculada não encontrada");
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnSuccess_When_TransactionIsFoundAndDeleted()
    {
        var request = new DeleteTransactionRequest { Id = 1, UserId = 123 };
        var existingTransaction = new Domain.Models.Transaction
        {
            Id = 1,
            UserId = 123,
            CategoryId = 10,
            Title = "A ser deletada"
        };

        _mockTransactionRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingTransaction);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(existingTransaction.CategoryId, request.UserId))
            .ReturnsAsync(new Domain.Models.Category());

        _mockTransactionRepo.Setup(r => r.DeleteAsync(It.IsAny<Domain.Models.Transaction>()))
            .ReturnsAsync(existingTransaction);

        var result = await _handler.DeleteAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Transação excluída com sucesso!");

        _mockTransactionRepo.Verify(r => r.GetByIdAsync(request.Id, request.UserId), Times.Once);
        _mockCategoryRepo.Verify(r => r.GetByIdAsync(existingTransaction.CategoryId, request.UserId), Times.Once);
        _mockTransactionRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Transaction>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnNotFound_When_TransactionDoesNotExist()
    {
        var request = new DeleteTransactionRequest { Id = 99, UserId = 123 };

        _mockTransactionRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync((Domain.Models.Transaction?)null);

        var result = await _handler.DeleteAsync(request);

        result.Data.Should().BeNull();
        result.Message.Should().Be("Transação não encontrada");
        _mockTransactionRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Transaction>()), Times.Never);
    }

    [Fact]
    public async Task GetByPeriodAsync_Should_ReturnPagedData_When_Successful()
    {
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = 123,
            PageNumber = 1,
            PageSize = 3
        };

        var transactionsFromDb = new List<Domain.Models.Transaction>
    {
        new() { Id = 1, Title = "Salário", Amount = 5000, UserId = 123, CategoryId = 1 },
        new() { Id = 2, Title = "Aluguel", Amount = 1500, UserId = 123, CategoryId = 2 },
        new() { Id = 3, Title = "Internet", Amount = 100, UserId = 123, CategoryId = 1 }
    };

        _mockTransactionRepo.Setup(r => r.GetByPeriodAsync(request.UserId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(transactionsFromDb);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), request.UserId))
            .ReturnsAsync(new Domain.Models.Category());

        var result = await _handler.GetByPeriodAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(request.PageSize);
        result.TotalCount.Should().Be(transactionsFromDb.Count);
    }
}