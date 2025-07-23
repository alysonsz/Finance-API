using Finance.Application.Commands.Transactions;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Responses;
using Finance.Contracts.Responses.Categories;
using Finance.Contracts.Responses.Transactions;
using Finance.Domain.Common;
using MediatR;

namespace Finance.Application.Handlers.Transactions;

public class GetTransactionReportQueryHandler(ITransactionRepository transactionRepository)
    : IRequestHandler<GetTransactionReportCommand, Response<TransactionReportResponse>>
{
    public async Task<Response<TransactionReportResponse>> Handle(GetTransactionReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var startDate = request.StartDate ?? DateTime.Now.GetFirstDay();
            var endDate = request.EndDate ?? DateTime.Now.GetLastDay();

            var transactions = await transactionRepository.GetAllByPeriodAsync(request.UserId, startDate, endDate);

            var expenses = transactions.Where(t => t.Amount < 0);
            var incomes = transactions.Where(t => t.Amount > 0);

            var report = new TransactionReportResponse
            {
                TotalExpenses = expenses.Sum(e => Math.Abs(e.Amount)),
                TotalIncomes = incomes.Sum(i => i.Amount),
                ExpensesByCategory = expenses
                    .GroupBy(e => e.Category?.Title ?? "Sem Categoria")
                    .Select(g => new CategorySummaryResponse
                    {
                        CategoryName = g.Key,
                        Total = Math.Abs(g.Sum(t => t.Amount))
                    }).ToList(),
                IncomesByCategory = incomes
                    .GroupBy(i => i.Category?.Title ?? "Sem Categoria")
                    .Select(g => new CategorySummaryResponse
                    {
                        CategoryName = g.Key,
                        Total = g.Sum(t => t.Amount)
                    }).ToList()
            };

            return new Response<TransactionReportResponse>(report, message: "Relatório gerado com sucesso.");
        }
        catch
        {
            return new Response<TransactionReportResponse>(null, 500, "Não foi possível gerar o relatório.");
        }
    }
}
