using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Customer
{
    public string Customerid { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Middlename { get; set; }

    public string? Firstname { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Dob { get; set; }

    public string? Gender { get; set; }

    public string? Metadata { get; set; }
    public string? Publicid { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual User CustomerNavigation { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
