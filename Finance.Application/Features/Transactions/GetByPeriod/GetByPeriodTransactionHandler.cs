using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Common;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.GetByPeriod;

public class GetByPeriodTransactionHandler(ITransactionRepository transactionRepository)
    : IRequestHandler<GetByPeriodTransactionCommand, PagedResponse<List<TransactionDto>?>>
{
    public async Task<PagedResponse<List<TransactionDto>?>> Handle(GetByPeriodTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var startDate = request.StartDate ?? DateTime.Now.GetFirstDay();
            var endDate = request.EndDate ?? DateTime.Now.GetLastDay();

            var totalCount = await transactionRepository.CountByPeriodAsync(request.UserId, startDate, endDate);

            var transactions = await transactionRepository.GetByPeriodAsync(
                request.UserId,
                startDate,
                endDate,
                request.PageNumber,
                request.PageSize);

            var dtos = transactions.Select(MapToDto).ToList();

            return new PagedResponse<List<TransactionDto>?>(
                dtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<TransactionDto>?>("Não foi possível obter as transações", 500);
        }
    }

    private static TransactionDto MapToDto(Transaction transaction)
        => new()
        {
            Id = transaction.Id,
            Title = transaction.Title,
            Amount = transaction.Amount,
            Type = transaction.Type,
            PaidOrReceivedAt = transaction.PaidOrReceivedAt,
            CreatedAt = transaction.CreatedAt,
            Category = new CategoryDto
            {
                Id = transaction.Category.Id,
                Title = transaction.Category.Title,
                Description = transaction.Category.Description
            }
        };
}
