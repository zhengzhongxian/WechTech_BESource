using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Permission
{
    public string Permissionid { get; set; } = null!;

    public string? Roleid { get; set; }

    public string? Actionid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual Action? Action { get; set; }

    public virtual Role? Role { get; set; }
}
