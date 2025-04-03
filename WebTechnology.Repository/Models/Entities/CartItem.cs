using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class CartItem
{
    public string Id { get; set; } = null!;

    public string? CartId { get; set; }

    public string? Productid { get; set; }

    public int? Quantity { get; set; }

    public virtual Cart? Cart { get; set; }
}
