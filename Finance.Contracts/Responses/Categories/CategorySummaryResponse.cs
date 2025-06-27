namespace Finance.Contracts.Responses.Categories;

public class CategorySummaryResponse
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
