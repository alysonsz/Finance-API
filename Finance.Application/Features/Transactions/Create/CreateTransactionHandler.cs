using Finance.Application.Interfaces.Repositories;
using Finance.Application.Mappers;
using Finance.Contracts.DTOs;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;

namespace Finance.Application.Features.Transactions.Create;

public class CreateTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<CreateTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = Transaction.Create(
            request.Title,
            request.Amount,
            request.Type,
            request.CategoryId,
            request.UserId,
            request.PaidOrReceivedAt);

        if (result.IsFailure)
            return Response<TransactionDto?>.Fail(string.Join("; ", result.Errors));

        var transaction = result.Value;

        try
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria não encontrada.");

            await transactionRepository.CreateAsync(transaction);

            var dto = TransactionMapper.ToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 201, "Transação criada com sucesso!");
        }
        catch
        {
            return Response<TransactionDto?>.Fail("Não foi possível criar a transação.");
        }
    }
}
