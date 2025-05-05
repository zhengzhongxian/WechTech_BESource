using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Voucher
{
    public string Voucherid { get; set; } = null!;

    public string? Code { get; set; }

    public decimal? DiscountValue { get; set; }

    public DiscountType? DiscountType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public decimal? MinOrder { get; set; }

    public decimal? MaxDiscount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<ApplyVoucher> ApplyVouchers { get; set; } = new List<ApplyVoucher>();
}
