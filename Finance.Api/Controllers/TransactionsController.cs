using Finance.API.Extensions;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Requests.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/transactions")]
public class TransactionsController(ITransactionHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTransactionRequest request)
    {
        var userId = User.GetUserId();
        request.UserId = userId;
        var response = await handler.CreateAsync(request);
        return this.FromResponse(response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(UpdateTransactionRequest request, [FromRoute] long id)
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
        var request = new DeleteTransactionRequest
        {
            UserId = userId,
            Id = id
        };
        var response = await handler.DeleteAsync(request);
        return this.FromResponse(response);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
    {
        var userId = User.GetUserId();
        var request = new GetTransactionByIdRequest
        {
            UserId = userId,
            Id = id
        };
        var response = await handler.GetByIdAsync(request);
        return this.FromResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByPeriodAsync(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25)
    {
        var userId = User.GetUserId();
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await handler.GetByPeriodAsync(request);
        return this.FromResponse(response);
    }
}