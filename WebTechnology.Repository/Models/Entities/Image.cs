using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Image
{
    public string Imageid { get; set; } = null!;

    public string? ImageData { get; set; }

    public string? Productid { get; set; }

    public string? Order { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }
    public string? Publicid { get; set; }

    public virtual Product? Product { get; set; }
}
