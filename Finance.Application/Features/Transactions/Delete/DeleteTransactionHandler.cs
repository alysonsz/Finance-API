using Finance.Application.Mappers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.Delete;

public class DeleteTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada.");

            var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria vinculada à transação não foi encontrada.");

            await transactionRepository.DeleteAsync(transaction);

            var dto = TransactionMapper.ToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 200, "Transação excluída com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível excluir a transação.");
        }
    }
}
