using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Dimension
{
    public string Dimensionid { get; set; } = null!;

    public string? Productid { get; set; }

    public decimal? WeightValue { get; set; }

    public decimal? HeightValue { get; set; }

    public decimal? WidthValue { get; set; }

    public decimal? LengthValue { get; set; }

    public string? Metadata { get; set; }

    public virtual Product? Product { get; set; }
}
