namespace Finance.Application.Requests.Transactions;

public class GetTransactionsByPeriodRequest : PagedRequest
{
    public long UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}