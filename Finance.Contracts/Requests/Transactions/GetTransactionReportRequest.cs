namespace Finance.Contracts.Requests.Transactions;

public class GetTransactionReportRequest
{
    public long UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

