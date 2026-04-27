using Finance.Application.Features.BankConnections.Sync;
using Finance.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FinanceNucleusController(IMediator mediator) : ControllerBase
{
    [HttpGet("dashboard/stats/{userId}")]
    public async Task<IActionResult> GetDashboardStats(long userId)
    {
        var result = await mediator.Send(new GetDashboardStatsQuery(userId));
        
        if (result.IsSuccess)
            return Ok(result);
            
        return BadRequest(result);
    }

    [HttpPost("bank-connections/{connectionId}/sync")]
    public async Task<IActionResult> SyncTransactions(string connectionId, [FromQuery] long userId)
    {
        var result = await mediator.Send(new SyncBankConnectionCommand(userId, connectionId));
        
        if (result.IsSuccess)
            return Ok(result);
            
        return BadRequest(result);
    }

    [HttpGet("transactions/flow/{userId}")]
    public async Task<IActionResult> GetTransactionFlow(long userId)
    {
        var result = await mediator.Send(new Finance.Application.Features.Transactions.Queries.GetTransactionFlowQuery(userId));
        
        if (result.IsSuccess)
            return Ok(result);
            
        return BadRequest(result);
    }
}
