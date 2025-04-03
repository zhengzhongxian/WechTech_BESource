using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Brand
{
    public string Brand1 { get; set; } = null!;

    public string? BrandName { get; set; }

    public string? LogoData { get; set; }

    public string? Website { get; set; }

    public string? ManufactureAddress { get; set; }

    public string? Country { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
