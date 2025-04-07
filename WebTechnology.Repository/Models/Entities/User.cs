using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class User
{
    public string Userid { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? Otp { get; set; }

    public bool? Authenticate { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? Roleid { get; set; }

    public string? PasswordResetToken { get; set; }

    public string? RefreshToken { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? StatusId { get; set; }

    public int CountAuth { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual Role? Role { get; set; }

    public virtual UserStatus? Status { get; set; }
}
