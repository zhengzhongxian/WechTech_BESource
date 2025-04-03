using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Unit
{
    public string Unit1 { get; set; } = null!;

    public string? UnitName { get; set; }

    public string? UnitSymbol { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
