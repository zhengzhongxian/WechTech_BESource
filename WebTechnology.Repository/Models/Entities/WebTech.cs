using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace WebTechnology.API;

public partial class WebTech : DbContext
{
    public WebTech()
    {
    }

    public WebTech(DbContextOptions<WebTech> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<ApplyVoucher> ApplyVouchers { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Dimension> Dimensions { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderLog> OrderLogs { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<ProductTrend> ProductTrends { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sysdiagram> Sysdiagrams { get; set; }

    public virtual DbSet<Trend> Trends { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Actionid).HasName("PRIMARY");

            entity.ToTable("actions");

            entity.Property(e => e.Actionid)
                .HasMaxLength(64)
                .HasColumnName("actionid");
            entity.Property(e => e.ActionName)
                .HasMaxLength(255)
                .HasColumnName("action_name");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ApplyVoucher>(entity =>
        {
            entity.HasKey(e => e.Applyid).HasName("PRIMARY");

            entity.ToTable("apply_vouchers");

            entity.HasIndex(e => e.Voucherid, "FK_apply_vouchers_vouchers");

            entity.HasIndex(e => new { e.Orderid, e.Voucherid }, "unique_order_voucher").IsUnique();

            entity.Property(e => e.Applyid)
                .HasMaxLength(64)
                .HasColumnName("applyid");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Orderid)
                .HasMaxLength(64)
                .HasColumnName("orderid");
            entity.Property(e => e.Voucherid)
                .HasMaxLength(64)
                .HasColumnName("voucherid");

            entity.HasOne(d => d.Order).WithMany(p => p.ApplyVouchers)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("FK_apply_vouchers_orders");

            entity.HasOne(d => d.Voucher).WithMany(p => p.ApplyVouchers)
                .HasForeignKey(d => d.Voucherid)
                .HasConstraintName("FK_apply_vouchers_vouchers");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Brand1).HasName("PRIMARY");

            entity.ToTable("brands");

            entity.Property(e => e.Brand1)
                .HasMaxLength(64)
                .HasColumnName("brand");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brand_name");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LogoData)
                .HasMaxLength(500)
                .HasColumnName("logo_data");
            entity.Property(e => e.ManufactureAddress)
                .HasMaxLength(500)
                .HasColumnName("manufacture_address");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Cartid).HasName("PRIMARY");

            entity.ToTable("carts");

            entity.Property(e => e.Cartid)
                .HasMaxLength(64)
                .HasColumnName("cartid");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CartNavigation).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.Cartid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_carts_customers");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cart_items");

            entity.HasIndex(e => new { e.CartId, e.Productid }, "unique_cart_product").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(64)
                .HasColumnName("id");
            entity.Property(e => e.CartId)
                .HasMaxLength(64)
                .HasColumnName("cart_id");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_cart_items_carts");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Parentid, "FK_categories_parents");

            entity.Property(e => e.Categoryid)
                .HasMaxLength(64)
                .HasColumnName("categoryid");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Parentid)
                .HasMaxLength(64)
                .HasColumnName("parentid");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Parent).WithMany(p => p.Categories)
                .HasForeignKey(d => d.Parentid)
                .HasConstraintName("FK_categories_parents");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Commentid).HasName("PRIMARY");

            entity.ToTable("comment");

            entity.HasIndex(e => e.Reviewid, "FK_comment_review");

            entity.Property(e => e.Commentid)
                .HasMaxLength(64)
                .HasColumnName("commentid");
            entity.Property(e => e.CommentText)
                .HasMaxLength(1000)
                .HasColumnName("comment_text");
            entity.Property(e => e.CommentedAt)
                .HasMaxLength(6)
                .HasColumnName("commented_at");
            entity.Property(e => e.ModifiedAt)
                .HasMaxLength(6)
                .HasColumnName("modified_at");
            entity.Property(e => e.Reviewid)
                .HasMaxLength(64)
                .HasColumnName("reviewid");

            entity.HasOne(d => d.Review).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Reviewid)
                .HasConstraintName("FK_comment_review");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("PRIMARY");

            entity.ToTable("customers");

            entity.Property(e => e.Customerid)
                .HasMaxLength(64)
                .HasColumnName("customerid");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Dob)
                .HasMaxLength(6)
                .HasColumnName("dob");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Middlename)
                .HasMaxLength(255)
                .HasColumnName("middlename");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");

            entity.HasOne(d => d.CustomerNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customers_users");
        });

        modelBuilder.Entity<Dimension>(entity =>
        {
            entity.HasKey(e => e.Dimensionid).HasName("PRIMARY");

            entity.ToTable("dimensions");

            entity.HasIndex(e => e.Productid, "FK_dimensions_products");

            entity.Property(e => e.Dimensionid)
                .HasMaxLength(64)
                .HasColumnName("dimensionid");
            entity.Property(e => e.HeightValue)
                .HasPrecision(18, 2)
                .HasColumnName("height_value");
            entity.Property(e => e.LengthValue)
                .HasPrecision(18, 2)
                .HasColumnName("length_value");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.WeightValue)
                .HasPrecision(18, 2)
                .HasColumnName("weight_value");
            entity.Property(e => e.WidthValue)
                .HasPrecision(18, 2)
                .HasColumnName("width_value");

            entity.HasOne(d => d.Product).WithMany(p => p.Dimensions)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_dimensions_products");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Imageid).HasName("PRIMARY");

            entity.ToTable("images");

            entity.HasIndex(e => e.Productid, "FK_images_products");

            entity.Property(e => e.Imageid)
                .HasMaxLength(64)
                .HasColumnName("imageid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageData)
                .HasMaxLength(0)
                .HasColumnName("image_data");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Order)
                .HasMaxLength(64)
                .HasColumnName("order");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_images_products");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Logid).HasName("PRIMARY");

            entity.ToTable("logs");

            entity.HasIndex(e => e.Userid, "FK_logs_users");

            entity.Property(e => e.Logid)
                .HasMaxLength(64)
                .HasColumnName("logid");
            entity.Property(e => e.Actionid)
                .HasMaxLength(64)
                .HasColumnName("actionid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DescriptionDetails)
                .HasMaxLength(255)
                .HasColumnName("description_details");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Useragent)
                .HasMaxLength(255)
                .HasColumnName("useragent");
            entity.Property(e => e.Userid)
                .HasMaxLength(64)
                .HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK_logs_users");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.StatusId, "FK_orders_order_status");

            entity.HasIndex(e => e.PaymentMethod, "FK_orders_payments");

            entity.Property(e => e.Orderid)
                .HasMaxLength(64)
                .HasColumnName("orderid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(64)
                .HasColumnName("customer_id");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsSuccess).HasColumnName("is_success");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasMaxLength(6)
                .HasColumnName("order_date");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("order_number");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(64)
                .HasColumnName("payment_method");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(500)
                .HasColumnName("shipping_address");
            entity.Property(e => e.ShippingCode)
                .HasMaxLength(50)
                .HasColumnName("shipping_code");
            entity.Property(e => e.ShippingFee)
                .HasPrecision(18, 2)
                .HasColumnName("shipping_fee");
            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(18, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethod)
                .HasConstraintName("FK_orders_payments");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_orders_order_status");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PRIMARY");

            entity.ToTable("order_details");

            entity.HasIndex(e => e.ProductId, "FK_order_details_products");

            entity.Property(e => e.OrderDetailId)
                .HasMaxLength(64)
                .HasColumnName("order_detail_id");
            entity.Property(e => e.OrderId)
                .HasMaxLength(64)
                .HasColumnName("order_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(64)
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_order_details_orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_order_details_products");
        });

        modelBuilder.Entity<OrderLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_log");

            entity.HasIndex(e => e.OrderId, "FK_order_log_orders");

            entity.Property(e => e.Id)
                .HasMaxLength(64)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.NewStatusId)
                .HasMaxLength(64)
                .HasColumnName("new_status_id");
            entity.Property(e => e.OldStatusId)
                .HasMaxLength(64)
                .HasColumnName("old_status_id");
            entity.Property(e => e.OrderId)
                .HasMaxLength(64)
                .HasColumnName("order_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLogs)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_order_log_orders");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("order_status");

            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.Parentid).HasName("PRIMARY");

            entity.ToTable("parents");

            entity.Property(e => e.Parentid)
                .HasMaxLength(64)
                .HasColumnName("parentid");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.ParentName)
                .HasMaxLength(255)
                .HasColumnName("parent_name");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.Property(e => e.Paymentid)
                .HasMaxLength(64)
                .HasColumnName("paymentid");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.PaymentName)
                .HasMaxLength(255)
                .HasColumnName("payment_name");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Permissionid).HasName("PRIMARY");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Actionid, "FK_permissions_actions");

            entity.HasIndex(e => new { e.Roleid, e.Actionid }, "unique_role_action").IsUnique();

            entity.Property(e => e.Permissionid)
                .HasMaxLength(64)
                .HasColumnName("permissionid");
            entity.Property(e => e.Actionid)
                .HasMaxLength(64)
                .HasColumnName("actionid");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Roleid)
                .HasMaxLength(64)
                .HasColumnName("roleid");

            entity.HasOne(d => d.Action).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Actionid)
                .HasConstraintName("FK_permissions_actions");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK_permissions_roles");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.Brand, "FK_products_brands");

            entity.HasIndex(e => e.StatusId, "FK_products_product_status");

            entity.HasIndex(e => e.Unit, "FK_products_units");

            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.Bar)
                .HasMaxLength(100)
                .HasColumnName("bar");
            entity.Property(e => e.Brand)
                .HasMaxLength(64)
                .HasColumnName("brand");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.Stockquantity).HasColumnName("stockquantity");
            entity.Property(e => e.Unit)
                .HasMaxLength(64)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.BrandNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Brand)
                .HasConstraintName("FK_products_brands");

            entity.HasOne(d => d.Status).WithMany(p => p.Products)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_products_product_status");

            entity.HasOne(d => d.UnitNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Unit)
                .HasConstraintName("FK_products_units");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_categories");

            entity.HasIndex(e => e.Categoryid, "FK_product_categories_categories");

            entity.HasIndex(e => new { e.Productid, e.Categoryid }, "unique_product_category").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(64)
                .HasColumnName("id");
            entity.Property(e => e.Categoryid)
                .HasMaxLength(64)
                .HasColumnName("categoryid");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("FK_product_categories_categories");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_product_categories_products");
        });

        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.HasKey(e => e.Ppsid).HasName("PRIMARY");

            entity.ToTable("product_prices");

            entity.HasIndex(e => e.Productid, "FK_product_prices_products");

            entity.Property(e => e.Ppsid)
                .HasMaxLength(64)
                .HasColumnName("ppsid");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_product_prices_products");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("product_status");

            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ProductTrend>(entity =>
        {
            entity.HasKey(e => e.Ptsid).HasName("PRIMARY");

            entity.ToTable("product_trends");

            entity.HasIndex(e => e.Trend, "FK_product_trends_trends");

            entity.HasIndex(e => new { e.Productid, e.Trend }, "unique_product_trend").IsUnique();

            entity.Property(e => e.Ptsid)
                .HasMaxLength(64)
                .HasColumnName("ptsid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate)
                .HasMaxLength(6)
                .HasColumnName("end_date");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.StartDate)
                .HasMaxLength(6)
                .HasColumnName("start_date");
            entity.Property(e => e.Trend)
                .HasMaxLength(64)
                .HasColumnName("trend");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTrends)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_product_trends_products");

            entity.HasOne(d => d.TrendNavigation).WithMany(p => p.ProductTrends)
                .HasForeignKey(d => d.Trend)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_product_trends_trends");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("PRIMARY");

            entity.ToTable("review");

            entity.HasIndex(e => e.Productid, "FK_review_products");

            entity.HasIndex(e => new { e.Customerid, e.Productid }, "unique_customer_product").IsUnique();

            entity.Property(e => e.Reviewid)
                .HasMaxLength(64)
                .HasColumnName("reviewid");
            entity.Property(e => e.Customerid)
                .HasMaxLength(64)
                .HasColumnName("customerid");
            entity.Property(e => e.Productid)
                .HasMaxLength(64)
                .HasColumnName("productid");
            entity.Property(e => e.Rate).HasColumnName("rate");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Customerid)
                .HasConstraintName("FK_review_customers");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("FK_review_products");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Roleid)
                .HasMaxLength(64)
                .HasColumnName("roleid");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Sysdiagram>(entity =>
        {
            entity.HasKey(e => e.DiagramId).HasName("PRIMARY");

            entity.ToTable("sysdiagrams");

            entity.HasIndex(e => new { e.PrincipalId, e.Name }, "UK_principal_name").IsUnique();

            entity.Property(e => e.DiagramId)
                .ValueGeneratedNever()
                .HasColumnName("diagram_id");
            entity.Property(e => e.Definition).HasColumnName("definition");
            entity.Property(e => e.Name)
                .HasMaxLength(160)
                .HasColumnName("name");
            entity.Property(e => e.PrincipalId).HasColumnName("principal_id");
            entity.Property(e => e.Version).HasColumnName("version");
        });

        modelBuilder.Entity<Trend>(entity =>
        {
            entity.HasKey(e => e.Trend1).HasName("PRIMARY");

            entity.ToTable("trends");

            entity.Property(e => e.Trend1)
                .HasMaxLength(64)
                .HasColumnName("trend");
            entity.Property(e => e.BannerData)
                .HasMaxLength(500)
                .HasColumnName("banner_data");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.TrendName)
                .HasMaxLength(255)
                .HasColumnName("trend_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Unit1).HasName("PRIMARY");

            entity.ToTable("units");

            entity.Property(e => e.Unit1)
                .HasMaxLength(64)
                .HasColumnName("unit");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.UnitName)
                .HasMaxLength(255)
                .HasColumnName("unit_name");
            entity.Property(e => e.UnitSymbol)
                .HasMaxLength(50)
                .HasColumnName("unit_symbol");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Roleid, "FK_users_roles");

            entity.HasIndex(e => e.StatusId, "FK_users_user_status");

            entity.HasIndex(e => e.Email, "unique_email").IsUnique();

            entity.Property(e => e.Userid)
                .HasMaxLength(64)
                .HasColumnName("userid");
            entity.Property(e => e.Authenticate).HasColumnName("authenticate");
            entity.Property(e => e.CountAuth).HasColumnName("count_Auth");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Otp)
                .HasMaxLength(255)
                .HasColumnName("otp");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PasswordResetToken)
                .HasMaxLength(255)
                .HasColumnName("password_reset_token");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(1024)
                .HasColumnName("refresh_token");
            entity.Property(e => e.Roleid)
                .HasMaxLength(64)
                .HasColumnName("roleid");
            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.VerifiedAt)
                .HasMaxLength(6)
                .HasColumnName("verified_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK_users_roles");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_users_user_status");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("user_status");

            entity.Property(e => e.StatusId)
                .HasMaxLength(64)
                .HasColumnName("status_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Voucherid).HasName("PRIMARY");

            entity.ToTable("vouchers");

            entity.Property(e => e.Voucherid)
                .HasMaxLength(64)
                .HasColumnName("voucherid");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(50)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(18, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasMaxLength(6)
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxDiscount)
                .HasPrecision(18, 2)
                .HasColumnName("max_discount");
            entity.Property(e => e.Metadata)
                .HasMaxLength(64)
                .HasColumnName("metadata");
            entity.Property(e => e.MinOrder)
                .HasPrecision(18, 2)
                .HasColumnName("min_order");
            entity.Property(e => e.StartDate)
                .HasMaxLength(6)
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount).HasColumnName("used_count");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
