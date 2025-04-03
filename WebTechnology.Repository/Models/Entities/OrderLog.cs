using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class OrderLog
{
    public string Id { get; set; } = null!;

    public string? OrderId { get; set; }

    public string? OldStatusId { get; set; }

    public string? NewStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Order? Order { get; set; }
}
