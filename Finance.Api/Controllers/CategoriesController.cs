using Finance.API.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/categories")]
public class CategoriesController(ICategoryHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request)
    {
        var userId = User.GetUserId();
        request.UserId = userId;
        var response = await handler.CreateAsync(request);
        return this.FromResponse(response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(UpdateCategoryRequest request, [FromRoute] long id)
    {
        var userId = User.GetUserId();
        request.UserId = userId;
        request.Id = id;
        var response = await handler.UpdateAsync(request);
        return this.FromResponse(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var userId = User.GetUserId();
        var request = new DeleteCategoryRequest
        {
            UserId = userId,
            Id = id
        };
        var response = await handler.DeleteAsync(request);
        return this.FromResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var userId = User.GetUserId();
        var request = new GetAllCategoriesRequest
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await handler.GetAllAsync(request);
        return this.FromResponse(response);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var userId = User.GetUserId();
        var request = new GetCategoryByIdRequest
        {
            UserId = userId,
            Id = id
        };
        var response = await handler.GetByIdAsync(request);
        return this.FromResponse(response);
    }
}