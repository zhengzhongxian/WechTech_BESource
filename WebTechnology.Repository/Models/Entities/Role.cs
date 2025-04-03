using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Role
{
    public string Roleid { get; set; } = null!;

    public string? RoleName { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
