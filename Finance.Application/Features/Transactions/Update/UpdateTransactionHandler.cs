using Finance.Application.Mappers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.Update;

public class UpdateTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var amount = request.Amount;
        if (request.Type == ETransactionType.Withdraw && amount > 0)
            amount *= -1;

        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada.");

            var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria não encontrada.");

            transaction.Title = request.Title;
            transaction.Type = request.Type;
            transaction.Amount = amount;
            transaction.CategoryId = request.CategoryId;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

            await transactionRepository.UpdateAsync(transaction);

            var dto = TransactionMapper.ToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 200, "Transação atualizada com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível atualizar a transação.");
        }
    }
}
