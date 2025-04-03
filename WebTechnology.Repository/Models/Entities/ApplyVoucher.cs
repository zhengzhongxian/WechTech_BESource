using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class ApplyVoucher
{
    public string Applyid { get; set; } = null!;

    public string? Orderid { get; set; }

    public string? Voucherid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
