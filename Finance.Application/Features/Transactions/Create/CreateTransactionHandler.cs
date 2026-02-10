using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;
using MediatR;

namespace Finance.Application.Features.Transactions.Create;

public class CreateTransactionHandler(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<CreateTransactionCommand, Response<TransactionDto?>>
{
    public async Task<Response<TransactionDto?>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var amount = request.Amount;
        if (request.Type == ETransactionType.Withdraw && amount > 0)
            amount *= -1;

        try
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria não encontrada.");

            var transaction = new Transaction
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                Title = request.Title,
                Amount = amount,
                Type = request.Type,
                PaidOrReceivedAt = request.PaidOrReceivedAt,
                CreatedAt = DateTime.UtcNow
            };

            await transactionRepository.CreateAsync(transaction);

            var dto = MapToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 201, "Transação criada com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível criar a transação.");
        }
    }

    private static TransactionDto MapToDto(Transaction transaction, Category category)
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
                Id = category.Id,
                Title = category.Title,
                Description = category.Description
            }
        };
}
