using Finance.Application.Commands.Transactions;
using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/transactions")]
public class TransactionsController(IMediator mediator, ITransactionHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTransactionCommand command)
    {
        command.UserId = User.GetUserId();

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Created($"v1/transactions/{response.Data?.Id}", response.Data)
            : BadRequest(response.Message);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTransactionCommand command, [FromRoute] long id)
    {
        command.Id = id;
        command.UserId = User.GetUserId();

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var command = new DeleteTransactionCommand
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
        var command = new GetTransactionByIdRequest
        {
            Id = id,
            UserId = User.GetUserId()
        };

        var response = await handler.GetByIdAsync(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetByPeriodAsync([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new GetTransactionsByPeriodCommand
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReportAsync([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var query = new GetTransactionReportCommand
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await mediator.Send(query);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}