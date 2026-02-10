using Finance.Contracts.Requests.Transactions;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Transactions;
using Finance.Domain.Models.DTOs;

namespace Finance.Contracts.Interfaces.Services;

public interface ITransactionService
{
    Task<Response<TransactionDto?>> CreateAsync(CreateTransactionRequest request);
    Task<Response<TransactionDto?>> UpdateAsync(UpdateTransactionRequest request);
    Task<Response<TransactionDto?>> DeleteAsync(DeleteTransactionRequest request);
    Task<Response<TransactionDto?>> GetByIdAsync(GetTransactionByIdRequest request);
    Task<PagedResponse<List<TransactionDto>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request);
    Task<Response<TransactionReportResponse>> GetReportAsync(GetTransactionReportRequest request);
}
