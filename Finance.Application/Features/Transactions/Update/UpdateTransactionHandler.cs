using Finance.Application.Interfaces.Repositories;
using Finance.Application.Mappers;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Transactions.Update;

public class UpdateTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada.");

            var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria não encontrada.");

            var updateResult = transaction.Update(
                request.Title,
                request.Amount,
                request.Type,
                request.CategoryId,
                request.PaidOrReceivedAt);

            if (updateResult.IsFailure)
                return Response<TransactionDto?>.Fail(string.Join("; ", updateResult.Errors));

            await transactionRepository.UpdateAsync(transaction);

            var dto = TransactionMapper.ToDto(transaction, category);
            return Response<TransactionDto?>.Success(dto, "Transação atualizada com sucesso!");
        }
        catch
        {
            return Response<TransactionDto?>.Fail("Não foi possível atualizar a transação.");
        }
    }
}
