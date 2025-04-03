using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Log
{
    public string Logid { get; set; } = null!;

    public string? Userid { get; set; }

    public string? Actionid { get; set; }

    public string? DescriptionDetails { get; set; }

    public string? IpAddress { get; set; }

    public string? Useragent { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
