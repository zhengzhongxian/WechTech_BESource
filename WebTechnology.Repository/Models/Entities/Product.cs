using System;
using System.Collections.Generic;

namespace WebTechnology.API;

public partial class Product
{
    public string Productid { get; set; } = null!;

    public string? ProductName { get; set; }

    public int? Stockquantity { get; set; }

    public string? Bar { get; set; }

    public string? Sku { get; set; }

    public string? Description { get; set; }

    public string? Brand { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Metadata { get; set; }

    public virtual Brand? BrandNavigation { get; set; }

    public virtual ICollection<Dimension> Dimensions { get; set; } = new List<Dimension>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<ProductTrend> ProductTrends { get; set; } = new List<ProductTrend>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ProductStatus? Status { get; set; }

    public virtual Unit? UnitNavigation { get; set; }
}
