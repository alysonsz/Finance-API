using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/categories")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryRequest request)
    {
        request.UserId = User.GetUserId();

        var response = await service.CreateAsync(request);

        return response.IsSuccess
            ? Created($"v1/categories/{response.Data?.Id}", response.Data)
            : BadRequest(response.Message);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCategoryRequest request)
    {
        request.Id = id;
        request.UserId = User.GetUserId();

        var response = await service.UpdateAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var request = new DeleteCategoryRequest
        {
            Id = id,
            UserId = User.GetUserId()
        };

        var response = await service.DeleteAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var request = new GetCategoryByIdRequest
        {
            Id = id,
            UserId = User.GetUserId()
        };

        var response = await service.GetByIdAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var request = new GetAllCategoriesRequest
        {
            UserId = User.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await service.GetAllAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
