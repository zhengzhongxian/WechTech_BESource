using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebTechnology.API;

public partial class Trend
{
    public string Trend1 { get; set; } = null!;
    public string? TrendName { get; set; }

    [StringLength(100, ErrorMessage = "Tên xu hướng không được vượt quá 100 ký tự")]
    public string? BannerData { get; set; }

    public bool? IsActive { get; set; }

    [Range(1, 10, ErrorMessage = "Độ ưu tiên phải từ 1 đến 10")]
    public int? Priority { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual ICollection<ProductTrend> ProductTrends { get; set; } = new List<ProductTrend>();
}
