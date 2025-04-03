using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Category
{
    public string Categoryid { get; set; } = null!;

    public string? CategoryName { get; set; }

    public int? Priority { get; set; }

    public string? Parentid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual Parent? Parent { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
