using Finance.Application.Features.Categories.Create;
using Finance.Application.Features.Categories.Delete;
using Finance.Application.Features.Categories.GetAll;
using Finance.Application.Features.Categories.GetById;
using Finance.Application.Features.Categories.Update;
using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Models;
using FluentAssertions;
using Moq;

namespace Finance.Application.Tests.Handlers;

public class CategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _repoMock = new();

    [Fact]
    public async Task Create_Should_Return201_When_Successful()
    {
        var handler = new CreateCategoryHandler(_repoMock.Object);

        var command = new CreateCategoryCommand
        {
            UserId = 123,
            Title = "Educação",
            Description = "Cursos"
        };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => c);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result._code.Should().Be(201);
        result.Message.Should().Be("Categoria criada com sucesso!");
        result.Data!.Title.Should().Be(command.Title);

        _repoMock.Verify(r => r.CreateAsync(It.Is<Category>(c =>
            c.UserId == command.UserId &&
            c.Title == command.Title &&
            c.Description == command.Description)), Times.Once);
    }

    [Fact]
    public async Task Create_Should_Return500_When_RepositoryThrows()
    {
        var handler = new CreateCategoryHandler(_repoMock.Object);

        var command = new CreateCategoryCommand { UserId = 123, Title = "Educação" };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().BeNull();
        result._code.Should().Be(500);
        result.Message.Should().Be("Não foi possível criar a categoria");
    }

    [Fact]
    public async Task Update_Should_Return200_When_Found()
    {
        var handler = new UpdateCategoryHandler(_repoMock.Object);

        var command = new UpdateCategoryCommand
        {
            Id = 1,
            UserId = 123,
            Title = "Casa (Editado)",
            Description = "Contas"
        };

        var catResult = Category.Create("Casa", "Antigo", 123);
        var existing = catResult.Value;
        existing.GetType().GetProperty("Id")?.SetValue(existing, 1L);

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync(existing);

        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => c);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(200);
        result.Message.Should().Be("Categoria atualizada com sucesso");
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Description.Should().Be(command.Description);

        _repoMock.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
            c.Id == command.Id &&
            c.UserId == command.UserId &&
            c.Title == command.Title &&
            c.Description == command.Description)), Times.Once);
    }

    [Fact]
    public async Task Update_Should_Return404_When_NotFound()
    {
        var handler = new UpdateCategoryHandler(_repoMock.Object);

        var command = new UpdateCategoryCommand { Id = 99, UserId = 123, Title = "X" };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync((Category?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().BeNull();
        result._code.Should().Be(404);
        result.Message.Should().Be("Categoria não encontrada ou não pertence ao usuário.");
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Should_Return200_When_Found()
    {
        var handler = new DeleteCategoryHandler(_repoMock.Object);

        var command = new DeleteCategoryCommand { Id = 1, UserId = 123 };
        var catResult = Category.Create("A ser deletada", null, 123);
        var existing = catResult.Value;
        existing.GetType().GetProperty("Id")?.SetValue(existing, 1L);

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync(existing);

        _repoMock.Setup(r => r.DeleteAsync(existing))
            .ReturnsAsync((Category c) => c);

        var result = await handler.Handle(command, CancellationToken.None);

        result._code.Should().Be(200);
        result.Message.Should().Be("Categoria excluída com sucesso!");
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(existing.Id);

        _repoMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task GetById_Should_Return404_When_NotFound()
    {
        var handler = new GetCategoryByIdHandler(_repoMock.Object);

        var command = new GetCategoryByIdCommand { Id = 99, UserId = 123 };

        _repoMock.Setup(r => r.GetByIdAsync(command.Id, command.UserId))
            .ReturnsAsync((Category?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().BeNull();
        result._code.Should().Be(404);
        result.Message.Should().Be("Categoria não encontrada ou não pertence ao usuário.");
    }

    [Fact]
    public async Task GetAll_Should_ReturnPagedData_When_Successful()
    {
        var handler = new GetAllCategoriesHandler(_repoMock.Object);

        var command = new GetAllCategoriesCommand { UserId = 123, PageNumber = 1, PageSize = 2 };

        var categories = new List<Category>
        {
            Category.Create("Casa", null, 123).Value,
            Category.Create("Saúde", null, 123).Value,
            Category.Create("Transporte", null, 123).Value
        };
        categories[0].GetType().GetProperty("Id")?.SetValue(categories[0], 1L);
        categories[1].GetType().GetProperty("Id")?.SetValue(categories[1], 2L);
        categories[2].GetType().GetProperty("Id")?.SetValue(categories[2], 3L);

        _repoMock.Setup(r => r.GetAllAsync(command.UserId))
            .ReturnsAsync(categories);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
        result.TotalCount.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(2);
    }
}
