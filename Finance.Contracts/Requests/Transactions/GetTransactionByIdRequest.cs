﻿namespace Finance.Contracts.Requests.Transactions;

public class GetTransactionByIdRequest : Request
{
    public long Id { get; set; }
}