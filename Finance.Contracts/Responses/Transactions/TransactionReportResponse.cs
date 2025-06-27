using Finance.Contracts.Responses.Categories;

namespace Finance.Contracts.Responses.Transactions;

public class TransactionReportResponse
{
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncomes { get; set; }

    public List<CategorySummaryResponse> ExpensesByCategory { get; set; } = [];
    public List<CategorySummaryResponse> IncomesByCategory { get; set; } = [];
}
