﻿using System.ComponentModel.DataAnnotations;
using Finance.Domain.Enums;
using Finance.Domain.Models;

namespace Finance.Application.Requests.Transactions;

public class CreateTransactionRequest : Request
{
    public new long UserId { get; set; }

    [Required(ErrorMessage = "Título inválido")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo inválido")]
    public ETransactionType Type { get; set; } = ETransactionType.Withdraw;

    [Required(ErrorMessage = "Valor inválido")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Categoria inválida")]
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "Data inválida")]
    public DateTime? PaidOrReceivedAt { get; set; }
}