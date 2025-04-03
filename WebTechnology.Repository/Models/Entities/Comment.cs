using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Comment
{
    public string Commentid { get; set; } = null!;

    public string? CommentText { get; set; }

    public string? Reviewid { get; set; }

    public DateTime? CommentedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Review? Review { get; set; }
}
