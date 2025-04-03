using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Action
{
    public string Actionid { get; set; } = null!;

    public string? ActionName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
