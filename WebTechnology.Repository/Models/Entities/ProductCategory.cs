using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class ProductCategory
{
    public string Id { get; set; } = null!;

    public string? Productid { get; set; }

    public string? Categoryid { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
