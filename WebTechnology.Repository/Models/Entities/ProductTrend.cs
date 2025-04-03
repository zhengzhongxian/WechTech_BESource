using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class ProductTrend
{
    public string Ptsid { get; set; } = null!;

    public string? Productid { get; set; }

    public string? Trend { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Trend? TrendNavigation { get; set; }
}
