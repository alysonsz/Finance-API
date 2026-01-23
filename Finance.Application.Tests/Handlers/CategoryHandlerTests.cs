using Finance.Application.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Categories;
using FluentAssertions;
using Moq;

namespace Finance.Application.Tests.Handlers;

public class CategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly CategoryHandler _handler;

    public CategoryHandlerTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _cacheMock = new Mock<ICacheService>();
        _handler = new CategoryHandler(_mockRepo.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnData_When_Successful()
    {
        var request = new CreateCategoryRequest { Title = "Educação", Description = "Cursos", UserId = 123 };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Domain.Models.Category>()))
            .ReturnsAsync((Domain.Models.Category cat) => cat);

        var result = await _handler.CreateAsync(request);

        result.Data.Should().NotBeNull();
        result.Data?.Title.Should().Be(request.Title);
        _mockRepo.Verify(r => r.CreateAsync(It.Is<Domain.Models.Category>(c => c.Title == request.Title)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnNullData_When_RepositoryThrowsException()
    {
        var request = new CreateCategoryRequest { Title = "Educação", Description = "Cursos", UserId = 123 };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Domain.Models.Category>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        var result = await _handler.CreateAsync(request);

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnPagedData_When_Successful()
    {
        var request = new GetAllCategoriesRequest { UserId = 123, PageNumber = 1, PageSize = 2 };
        var categoriesFromDb = new List<Domain.Models.Category>
        {
            new() { Id = 1, Title = "Casa", UserId = 123 },
            new() { Id = 2, Title = "Saúde", UserId = 123 },
            new() { Id = 3, Title = "Transporte", UserId = 123 }
        };

        _mockRepo.Setup(r => r.GetAllAsync(request.UserId)).ReturnsAsync(categoriesFromDb);

        var result = await _handler.GetAllAsync(request);

        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(request.PageSize);
        result.TotalCount.Should().Be(categoriesFromDb.Count);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnNullData_When_RepositoryThrowsException()
    {
        var request = new GetAllCategoriesRequest { UserId = 123 };
        _mockRepo.Setup(r => r.GetAllAsync(request.UserId)).ThrowsAsync(new Exception("Erro simulado"));

        var result = await _handler.GetAllAsync(request);

        result.Data.Should().BeNull();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnSuccess_When_CategoryIsFoundAndUpdated()
    {
        var request = new UpdateCategoryRequest
        {
            Id = 1,
            Title = "Casa (Editado)",
            Description = "Contas de casa (Editado)",
            UserId = 123
        };

        var existingCategory = new Domain.Models.Category
        {
            Id = 1,
            Title = "Casa",
            Description = "Contas de casa",
            UserId = 123
        };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingCategory);

        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Domain.Models.Category>()))
            .ReturnsAsync((Domain.Models.Category cat) => cat);

        var result = await _handler.UpdateAsync(request);

        result.Data.Should().NotBeNull();
        result.Data?.Title.Should().Be(request.Title);
        result.Data?.Description.Should().Be(request.Description);

        _mockRepo.Verify(r => r.GetByIdAsync(request.Id, request.UserId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Category>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnNotFound_When_CategoryDoesNotExist()
    {
        var request = new UpdateCategoryRequest
        {
            Id = 99,
            Title = "Inexistente",
            Description = "Testando",
            UserId = 123
        };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync((Domain.Models.Category?)null);

        var result = await _handler.UpdateAsync(request);

        result.Data.Should().BeNull();
        result.Message.Should().Be("Categoria não encontrada");

        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnSuccess_When_CategoryIsFoundAndDeleted()
    {
        var request = new DeleteCategoryRequest { Id = 1, UserId = 123 };
        var existingCategory = new Domain.Models.Category { Id = 1, UserId = 123, Title = "A ser deletada" };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingCategory);

        _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Domain.Models.Category>()))
            .ReturnsAsync(existingCategory);

        var result = await _handler.DeleteAsync(request);

        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Categoria excluída com sucesso!");
        _mockRepo.Verify(r => r.GetByIdAsync(request.Id, request.UserId), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Category>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnNotFound_When_CategoryDoesNotExist()
    {

        var request = new DeleteCategoryRequest { Id = 99, UserId = 123 };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync((Domain.Models.Category?)null);

        var result = await _handler.DeleteAsync(request);

        result.Data.Should().BeNull();
        result.Message.Should().Be("Categoria não encontrada");
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Category>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnData_When_CategoryIsFound()
    {
        var request = new GetCategoryByIdRequest { Id = 1, UserId = 123 };
        var existingCategory = new Domain.Models.Category { Id = 1, UserId = 123, Title = "Categoria Encontrada" };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync(existingCategory);

        var result = await _handler.GetByIdAsync(request);

        result.Data.Should().NotBeNull();
        result.Data?.Id.Should().Be(request.Id);
        result.Data?.Title.Should().Be(existingCategory.Title);
        _mockRepo.Verify(r => r.GetByIdAsync(request.Id, request.UserId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNotFound_When_CategoryDoesNotExist()
    {
        var request = new GetCategoryByIdRequest { Id = 99, UserId = 123 };

        _mockRepo.Setup(r => r.GetByIdAsync(request.Id, request.UserId))
            .ReturnsAsync((Domain.Models.Category?)null);

        var result = await _handler.GetByIdAsync(request);

        result.Data.Should().BeNull();
        result.Message.Should().Be("Categoria não encontrada");
    }
}