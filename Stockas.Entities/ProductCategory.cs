using System;
using System.Collections.Generic;

namespace Stockas.Entities;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User User { get; set; } = null!;
}
