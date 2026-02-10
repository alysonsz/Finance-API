using Finance.Api.Tests.Integration;
using Finance.Application.Features.Categories.Create;
using Finance.Contracts.Requests.Categories;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Finance.Api.Tests.Controllers;

[Collection("Shared Test Collection")]
public class CategoriesControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task CreateCategory_Should_ReturnOk_When_RequestIsValid()
    {
        await AuthenticateAsync();

        var request = new CreateCategoryCommand
        {
            Title = "Viagens (Integration Test)",
            Description = "Despesas com viagens e passeios"
        };

        var response = await _client.PostAsJsonAsync("v1/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateCategory_Should_ReturnOk_When_RequestIsValid()
    {
        await AuthenticateAsync();

        var createRequest = new CreateCategoryCommand
        {
            Title = "Para Atualizar",
            Description = "Descrição"
        };

        var createResponse = await _client.PostAsJsonAsync("v1/categories", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryDto>(_options);
        createdCategory.Should().NotBeNull();

        var request = new UpdateCategoryRequest
        {
            Title = "Alimentação (Editado)",
            Description = "Editado via teste de integração"
        };

        var response = await _client.PutAsJsonAsync($"v1/categories/{createdCategory!.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCategory_Should_ReturnOk_When_CategoryExists()
    {
        await AuthenticateAsync();

        var createRequest = new CreateCategoryCommand
        {
            Title = "Para Deletar",
            Description = "Descrição"
        };

        var createResponse = await _client.PostAsJsonAsync("v1/categories", createRequest);

        var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryDto>(_options);
        createdCategory.Should().NotBeNull();

        long categoryIdToDelete = createdCategory.Id;

        var response = await _client.DeleteAsync($"v1/categories/{categoryIdToDelete}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllCategories_Should_ReturnOk_With_PagedData()
    {
        await AuthenticateAsync();

        await _client.PostAsJsonAsync("v1/categories", new CreateCategoryCommand
        {
            Title = $"Seed Cat {Guid.NewGuid()}",
            Description = "Seed"
        });

        int pageNumber = 1;
        int pageSize = 25;

        var response = await _client.GetAsync($"v1/categories?pageNumber={pageNumber}&pageSize={pageSize}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>(_options);

        if (content.ValueKind == JsonValueKind.Array)
        {
            content.GetArrayLength().Should().BeGreaterThan(0);
        }
        else
        {
            content.GetProperty("data").GetArrayLength().Should().BeGreaterThan(0);
        }
    }
}