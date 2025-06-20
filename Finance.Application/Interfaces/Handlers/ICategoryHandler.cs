using Finance.Application.Requests.Categories;
using Finance.Application.Responses;
using Finance.Domain.Models.DTOs;

namespace Finance.Application.Interfaces.Handlers;

public interface ICategoryHandler
{
    Task<Response<CategoryDto?>> CreateAsync(CreateCategoryRequest request);
    Task<Response<CategoryDto?>> UpdateAsync(UpdateCategoryRequest request);
    Task<Response<CategoryDto?>> DeleteAsync(DeleteCategoryRequest request);
    Task<Response<CategoryDto?>> GetByIdAsync(GetCategoryByIdRequest request);
    Task<PagedResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request);
}