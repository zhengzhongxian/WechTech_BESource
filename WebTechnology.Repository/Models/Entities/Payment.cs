using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Payment
{
    public string Paymentid { get; set; } = null!;

    public string? PaymentName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
