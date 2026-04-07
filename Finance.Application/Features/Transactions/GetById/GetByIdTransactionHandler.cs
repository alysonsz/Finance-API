using Finance.Application.Interfaces.Repositories;
using Finance.Application.Mappers;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Transactions.GetById;

public class GetByIdTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<GetByIdTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(GetByIdTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada.");

            var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria vinculada à transação não foi encontrada.");

            var dto = MapToDto(transaction, category);
            return Response<TransactionDto?>.Success(dto);
        }
        catch
        {
            return Response<TransactionDto?>.Fail("Não foi possível recuperar a transação.");
        }
    }

    private static TransactionDto MapToDto(Transaction transaction, Category category)
        => new()
        {
            Id = transaction.Id,
            Title = transaction.Title,
            Amount = transaction.Amount,
            Type = transaction.Type.ToString(),
            PaidOrReceivedAt = transaction.PaidOrReceivedAt,
            CreatedAt = transaction.CreatedAt,
            CategoryId = transaction.CategoryId,
            CategoryTitle = category.Title
        };
}
