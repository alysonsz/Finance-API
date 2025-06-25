namespace Finance.Contracts.Requests.Transactions;

public class DeleteTransactionRequest : Request
{
    public long Id { get; set; }
    public new long UserId { get; set; }
}