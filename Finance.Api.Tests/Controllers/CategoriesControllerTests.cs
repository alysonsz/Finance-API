using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;
using FluentAssertions;
using System.Net;

namespace Finance.Api.Tests.Controllers
{
    [Collection("Shared Test Collection")]
    public class CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task CreateCategory_Should_ReturnOk_When_RequestIsValid()
        {
            var request = new CreateCategoryRequest
            {
                Title = "Viagens (Integration Test)",
                Description = "Despesas com viagens e passeios"
            };

            var response = await _client.PostAsJsonAsync("v1/categories", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateCategory_Should_ReturnOk_When_RequestIsValid()
        {
            var createRequest = new CreateCategoryRequest { Title = "Para Atualizar", Description = "Descrição" };
            var createResponse = await _client.PostAsJsonAsync("v1/categories", createRequest);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();

            createdCategory.Should().NotBeNull();
            createdCategory!.Data.Should().NotBeNull();
            var categoryId = createdCategory.Data.Id;

            var request = new UpdateCategoryRequest
            {
                Id = categoryId,
                Title = "Alimentação (Editado)",
                Description = "Editado via teste de integração"
            };

            var response = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteCategory_Should_ReturnOk_When_CategoryExists()
        {
            var createRequest = new CreateCategoryRequest { Title = "Para Deletar", Description = "Descrição" };
            var createResponse = await _client.PostAsJsonAsync("v1/categories", createRequest);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<Response<CategoryDto>>();

            createdCategory.Should().NotBeNull();
            createdCategory!.Data.Should().NotBeNull();
            long categoryIdToDelete = createdCategory.Data.Id;

            var response = await _client.DeleteAsync($"v1/categories/{categoryIdToDelete}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllCategories_Should_ReturnOk_With_PagedData()
        {
            int pageNumber = 1;
            int pageSize = 25;

            var response = await _client.GetAsync($"v1/categories?pageNumber={pageNumber}&pageSize={pageSize}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<List<CategoryDto>>>();
            pagedResponse.Should().NotBeNull();
            pagedResponse.Data.Should().NotBeNull();
            pagedResponse.Data.Count.Should().BeGreaterThan(0);
        }
    }
}