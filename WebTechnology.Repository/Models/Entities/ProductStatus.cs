using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class ProductStatus
{
    public string StatusId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
