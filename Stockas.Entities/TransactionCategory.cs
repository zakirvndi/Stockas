using System;
using System.Collections.Generic;

namespace Stockas.Entities;

public partial class TransactionCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
