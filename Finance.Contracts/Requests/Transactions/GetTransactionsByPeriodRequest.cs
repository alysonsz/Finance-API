namespace Finance.Contracts.Requests.Transactions;

public class GetTransactionsByPeriodRequest : PagedRequest
{
    public new long UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}