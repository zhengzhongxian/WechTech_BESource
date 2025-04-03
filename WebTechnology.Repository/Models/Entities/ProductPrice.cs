using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class ProductPrice
{
    public string Ppsid { get; set; } = null!;

    public string? Productid { get; set; }

    public decimal? Price { get; set; }

    public bool? IsDefault { get; set; }

    public virtual Product? Product { get; set; }
}
