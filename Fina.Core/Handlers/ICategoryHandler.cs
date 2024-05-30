using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fina.Core.Handlers
{
    public interface ICategoryHandler
    {
        Task<Response<Category?>> CreateAsync(CreateCategoryRequest request);
        Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request);
        Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request);
        Task<Response<Category?>> GetByIdAsync(GetCategoryByldRequest request);
        Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request);

    }
}
