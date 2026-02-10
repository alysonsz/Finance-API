using Finance.Application.Extensions;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[Authorize]
[ApiController]
[Route("v1/transactions")]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTransactionRequest request)
    {
        request.UserId = User.GetUserId();

        var response = await service.CreateAsync(request);

        return response.IsSuccess
            ? Created($"v1/transactions/{response.Data?.Id}", response.Data)
            : BadRequest(response.Message);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateTransactionRequest request)
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
        var request = new DeleteTransactionRequest
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
        var request = new GetTransactionByIdRequest
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
    public async Task<IActionResult> GetByPeriodAsync(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25)
    {
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await service.GetByPeriodAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReportAsync([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var request = new GetTransactionReportRequest
        {
            UserId = User.GetUserId(),
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await service.GetReportAsync(request);

        return response.IsSuccess
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
