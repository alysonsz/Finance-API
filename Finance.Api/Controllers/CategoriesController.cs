using Finance.Application.Interfaces.Handlers;
using Finance.Application.Requests.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/categories")]
public class CategoriesController(ICategoryHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request)
    {
        request.UserId = ApiConfiguration.UserId;
        var response = await handler.CreateAsync(request);
        return response.IsSuccess
            ? Created($"v1/categories/{response.Data?.Id}", response)
            : BadRequest(response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(UpdateCategoryRequest request, [FromRoute] long id)
    {
        request.UserId = ApiConfiguration.UserId;
        request.Id = id;
        var response = await handler.UpdateAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var request = new DeleteCategoryRequest
        {
            UserId = ApiConfiguration.UserId,
            Id = id
        };
        var response = await handler.DeleteAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var request = new GetAllCategoriesRequest
        {
            UserId = ApiConfiguration.UserId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await handler.GetAllAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var request = new GetCategoryByIdRequest
        {
            UserId = ApiConfiguration.UserId,
            Id = id
        };
        var response = await handler.GetByIdAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}