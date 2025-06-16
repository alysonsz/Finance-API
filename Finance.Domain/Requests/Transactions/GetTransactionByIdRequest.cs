namespace Finance.Domain.Requests.Transactions;

public class GetTransactionByIdRequest : Request
{
    public long Id { get; set; }
}