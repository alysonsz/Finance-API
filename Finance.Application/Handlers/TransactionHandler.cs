using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Domain.Common;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Domain.Models.DTOs;

namespace Finance.Application.Handlers;

public class TransactionHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository) : ITransactionHandler
{
    public async Task<Response<TransactionDto?>> CreateAsync(CreateTransactionRequest request)
    {
        request.Amount = Math.Abs(request.Amount);

        if (request.Type == ETransactionType.Withdraw && request.Amount >= 0)
            request.Amount *= -1;

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, request.UserId);
        if (category is null)
            return new Response<TransactionDto?>(null, 404, "Categoria não encontrada");

        var transaction = new Transaction
        {
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Amount = request.Amount,
            Type = request.Type,
            PaidOrReceivedAt = request.PaidOrReceivedAt,
            CreatedAt = DateTime.Now
        };

        try
        {
            await transactionRepository.CreateAsync(transaction);
            var dto = MapToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 201, "Transação criada com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível criar sua transação");
        }
    }

    public async Task<Response<TransactionDto?>> UpdateAsync(UpdateTransactionRequest request)
    {
        if (request.Type == ETransactionType.Withdraw && request.Amount >= 0)
            request.Amount *= -1;

        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada");

            transaction.CategoryId = request.CategoryId;
            transaction.Amount = request.Amount;
            transaction.Title = request.Title;
            transaction.Type = request.Type;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

            await transactionRepository.UpdateAsync(transaction);

            var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, transaction.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria vinculada não encontrada");

            var dto = MapToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 200, "Transação atualizada com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível atualizar sua transação");
        }
    }

    public async Task<Response<TransactionDto?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada");

            var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, transaction.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria vinculada não encontrada");

            await transactionRepository.DeleteAsync(transaction);

            var dto = MapToDto(transaction, category);
            return new Response<TransactionDto?>(dto, 200, "Transação excluída com sucesso!");
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível excluir sua transação");
        }
    }

    public async Task<Response<TransactionDto?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null)
                return new Response<TransactionDto?>(null, 404, "Transação não encontrada");

            var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, transaction.UserId);
            if (category is null)
                return new Response<TransactionDto?>(null, 404, "Categoria vinculada não encontrada");

            var dto = MapToDto(transaction, category);
            return new Response<TransactionDto?>(dto);
        }
        catch
        {
            return new Response<TransactionDto?>(null, 500, "Não foi possível recuperar sua transação");
        }
    }

    public async Task<PagedResponse<List<TransactionDto>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();

            var allTransactions = await transactionRepository.GetByPeriodAsync(request.UserId, request.StartDate, request.EndDate);
            if (allTransactions == null)
                return new PagedResponse<List<TransactionDto>?>(message: "Não foi possível obter as transações", code: 500);

            var pagedTransactions = allTransactions
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = new List<TransactionDto>();
            foreach (var t in pagedTransactions)
            {
                var category = await categoryRepository.GetByIdAsync(t.CategoryId, t.UserId);
                if (category is null) continue;

                dtos.Add(MapToDto(t, category));
            }

            return new PagedResponse<List<TransactionDto>?>(dtos, allTransactions.Count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<TransactionDto>?>(message: "Não foi possível obter as transações", code: 500);
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
