using System;
using System.Collections.Generic;

namespace Stockas.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int CategoryId { get; set; }

    public int? ProductId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
