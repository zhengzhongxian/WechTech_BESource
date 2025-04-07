using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Trend
{
    public string Trend1 { get; set; } = null!;

    public string? TrendName { get; set; }

    public string? BannerData { get; set; }

    public bool? IsActive { get; set; }

    public int? Priority { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<ProductTrend> ProductTrends { get; set; } = new List<ProductTrend>();
}
