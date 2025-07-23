using Finance.Application.Commands.Categories;
using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/categories")]
public class CategoriesController(IMediator mediator, ICategoryHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCategoryCommand request)
    {
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Created($"v1/categories/{response.Data?.Id}", response.Data)
            : BadRequest(response.Message);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            UserId = User.GetUserId()
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var command = new DeleteCategoryCommand
        {
            Id = id,
            UserId = User.GetUserId()
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var command = new GetCategoryByIdCommand
        {
            Id = id,
            UserId = User.GetUserId()
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var command = new GetAllCategoriesCommand
        {
            UserId = User.GetUserId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}