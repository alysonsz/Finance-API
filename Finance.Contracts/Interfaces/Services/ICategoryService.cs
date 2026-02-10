using Finance.Contracts.Requests.Categories;
using Finance.Contracts.Responses;
using Finance.Domain.Models.DTOs;

namespace Finance.Contracts.Interfaces.Services;

public interface ICategoryService
{
    Task<Response<CategoryDto?>> CreateAsync(CreateCategoryRequest request);
    Task<Response<CategoryDto?>> UpdateAsync(UpdateCategoryRequest request);
    Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request);
    Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request);
    Task<PagedResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request);
}
