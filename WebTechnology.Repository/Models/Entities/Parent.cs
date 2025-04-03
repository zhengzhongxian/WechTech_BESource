using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Parent
{
    public string Parentid { get; set; } = null!;

    public string? ParentName { get; set; }

    public int? Priority { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
