using Finance.Application.Interfaces.Handlers;
using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Common;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Domain.Requests.Transactions;
using Finance.Domain.Responses;

namespace Finance.Application.Handlers;

public class TransactionHandler(ITransactionRepository repository) : ITransactionHandler
{
    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 })
            request.Amount *= -1;

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
            await repository.CreateAsync(transaction);
            return new Response<Transaction?>(transaction, 201, "Transação criada com sucesso!");
        }
        catch
        {
            return new Response<Transaction?>(null, 500, "Não foi possível criar sua transação");
        }
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 }) request.Amount *= -1;

        try
        {
            var transaction = await repository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null) return new Response<Transaction?>(null, 404, "Transação não encontrada");

            transaction.CategoryId = request.CategoryId;
            transaction.Amount = request.Amount;
            transaction.Title = request.Title;
            transaction.Type = request.Type;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

            await repository.UpdateAsync(transaction);
            return new Response<Transaction?>(transaction, 200, "Transação atualizada com sucesso!");
        }
        catch { return new Response<Transaction?>(null, 500, "Não foi possível atualizar sua transação"); }
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await repository.GetByIdAsync(request.Id, request.UserId);
            if (transaction is null) return new Response<Transaction?>(null, 404, "Transação não encontrada");

            await repository.DeleteAsync(transaction);
            return new Response<Transaction?>(transaction, 200, "Transação excluída com sucesso!");
        }
        catch { return new Response<Transaction?>(null, 500, "Não foi possível excluir sua transação"); }
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await repository.GetByIdAsync(request.Id, request.UserId);
            return transaction is null
                ? new Response<Transaction?>(null, 404, "Transação não encontrada")
                : new Response<Transaction?>(transaction);
        }
        catch { return new Response<Transaction?>(null, 500, "Não foi possível recuperar sua transação"); }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();

            var allTransactions = await repository.GetByPeriodAsync(request.UserId, request.StartDate, request.EndDate);

            if (allTransactions == null)
            {
                return new PagedResponse<List<Transaction>?>("Não foi possível obter as transações", 500);
            }

            var pagedData = allTransactions.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

            return new PagedResponse<List<Transaction>?>(pagedData, allTransactions.Count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Transaction>?>("Não foi possível obter as transações", 500);
        }
    }
}