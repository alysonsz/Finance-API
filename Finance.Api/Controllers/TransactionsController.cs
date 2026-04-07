using Finance.Application.Extensions;
using Finance.Application.Features.Transactions.Create;
using Finance.Application.Features.Transactions.Delete;
using Finance.Application.Features.Transactions.GetById;
using Finance.Application.Features.Transactions.GetByPeriod;
using Finance.Application.Features.Transactions.GetReport;
using Finance.Application.Features.Transactions.Update;
using Finance.Contracts.Requests.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/transactions")]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTransactionRequest request)
    {
        var command = new CreateTransactionCommand
        {
            Title = request.Title,
            Amount = request.Amount,
            Type = request.Type,
            CategoryId = request.CategoryId,
            PaidOrReceivedAt = request.PaidOrReceivedAt,
            UserId = User.GetUserId()
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Created($"v1/transactions/{response.Data?.Id}", response.Data)
            : BadRequest(response.Message);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateTransactionRequest request)
    {
        var command = new UpdateTransactionCommand
        {
            Id = id,
            Title = request.Title,
            Amount = request.Amount,
            Type = request.Type,
            CategoryId = request.CategoryId,
            PaidOrReceivedAt = request.PaidOrReceivedAt,
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
        var command = new GetByIdTransactionCommand
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
    public async Task<IActionResult> GetByPeriodAsync(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25)
    {
        var command = new GetByPeriodTransactionCommand
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReportAsync([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var command = new GetReportTransactionCommand
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await mediator.Send(command);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
