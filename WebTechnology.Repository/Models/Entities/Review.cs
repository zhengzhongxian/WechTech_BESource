using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Review
{
    public string Reviewid { get; set; } = null!;

    public string? Customerid { get; set; }

    public string? Productid { get; set; }

    public int? Rate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
