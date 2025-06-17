using Finance.Application.Interfaces.Handlers;
using Finance.Domain.Requests.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("v1/transactions")]
public class TransactionsController(ITransactionHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTransactionRequest request)
    {
        request.UserId = ApiConfiguration.UserId;
        var response = await handler.CreateAsync(request);
        return response.IsSuccess
            ? Created($"v1/transactions/{response.Data?.Id}", response)
            : BadRequest(response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(UpdateTransactionRequest request, [FromRoute] long id)
    {
        request.UserId = ApiConfiguration.UserId;
        request.Id = id;
        var response = await handler.UpdateAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id)
    {
        var request = new DeleteTransactionRequest
        {
            UserId = ApiConfiguration.UserId,
            Id = id
        };
        var response = await handler.DeleteAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var request = new GetTransactionByIdRequest
        {
            UserId = ApiConfiguration.UserId,
            Id = id
        };
        var response = await handler.GetByIdAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByPeriodAsync(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25)
    {
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = ApiConfiguration.UserId,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await handler.GetByPeriodAsync(request);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}