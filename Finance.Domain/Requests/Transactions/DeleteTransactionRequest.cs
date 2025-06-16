namespace Finance.Domain.Requests.Transactions;

public class DeleteTransactionRequest : Request
{
    public long Id { get; set; }
}