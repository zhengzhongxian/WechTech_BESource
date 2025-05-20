using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Order
{
    public string Orderid { get; set; } = null!;

    public string? OrderNumber { get; set; }

    public string? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? ShippingAddress { get; set; }

    public decimal? ShippingFee { get; set; }

    public string? ShippingCode { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? StatusId { get; set; }

    public bool? IsSuccess { get; set; }

    public string? PaymentLinkId { get; set; }

    public virtual ICollection<ApplyVoucher> ApplyVouchers { get; set; } = new List<ApplyVoucher>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderLog> OrderLogs { get; set; } = new List<OrderLog>();

    public virtual Payment? PaymentMethodNavigation { get; set; }

    public virtual OrderStatus? Status { get; set; }
}
