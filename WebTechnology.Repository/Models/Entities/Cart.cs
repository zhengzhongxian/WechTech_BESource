using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Cart
{
    public string Cartid { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Customer CartNavigation { get; set; } = null!;
}
