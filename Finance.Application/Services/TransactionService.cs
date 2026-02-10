using Finance.Application.Features.Transactions.Create;
using Finance.Application.Features.Transactions.Delete;
using Finance.Application.Features.Transactions.GetById;
using Finance.Application.Features.Transactions.GetByPeriod;
using Finance.Application.Features.Transactions.GetReport;
using Finance.Application.Features.Transactions.Update;
using Finance.Contracts.Interfaces.Services;
using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Transactions;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Services;

public sealed class TransactionService(IMediator mediator) : ITransactionService
{
    private readonly IMediator _mediator = mediator;

    public Task<Response<TransactionDto?>> CreateAsync(CreateTransactionRequest request)
        => _mediator.Send(new CreateTransactionCommand
        {
            UserId = request.UserId,
            Title = request.Title,
            Type = request.Type,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            PaidOrReceivedAt = request.PaidOrReceivedAt
        });

    public Task<Response<TransactionDto?>> UpdateAsync(UpdateTransactionRequest request)
        => _mediator.Send(new UpdateTransactionCommand
        {
            Id = request.Id,
            UserId = request.UserId,
            Title = request.Title,
            Type = request.Type,
            Amount = request.Amount,
            CategoryId = request.CategoryId,
            PaidOrReceivedAt = request.PaidOrReceivedAt
        });

    public Task<Response<TransactionDto?>> DeleteAsync(DeleteTransactionRequest request)
        => _mediator.Send(new DeleteTransactionCommand
        {
            Id = request.Id,
            UserId = request.UserId
        });

    public Task<Response<TransactionDto?>> GetByIdAsync(GetTransactionByIdRequest request)
        => _mediator.Send(new GetByIdTransactionCommand
        {
            Id = request.Id,
            UserId = request.UserId
        });

    public Task<PagedResponse<List<TransactionDto>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
        => _mediator.Send(new GetByPeriodTransactionCommand
        {
            UserId = request.UserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });

    public Task<Response<TransactionReportResponse>> GetReportAsync(GetTransactionReportRequest request)
        => _mediator.Send(new GetReportTransactionCommand
        {
            UserId = request.UserId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        });
}
