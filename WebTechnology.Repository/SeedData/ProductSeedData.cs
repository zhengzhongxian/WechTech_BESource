using WebTechnology.API;

namespace WebTechnology.Repository.SeedData
{
    public static class ProductSeedData
    {
        public static List<Product> GetProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Productid = "P001",
                    ProductName = "Premium Laptop",
                    Stockquantity = 50,
                    Bar = "LAPTOP001",
                    Sku = "SKU001",
                    Description = "High-performance laptop with latest processor",
                    Brand = "B001",
                    Unit = "U001",
                    IsActive = true,
                    IsDeleted = false,
                    StatusId = "PS001",
                    Metadata = "Laptop",
                    CreatedAt = DateTime.UtcNow,
                    Dimensions = new List<Dimension>
                    {
                        new Dimension
                        {
                            Dimensionid = "D001",
                            Productid = "P001",
                            WeightValue = 1.5m,
                            HeightValue = 2.0m,
                            WidthValue = 15.0m,
                            LengthValue = 10.0m
                        }
                    },
                    ProductPrices = new List<ProductPrice>
                    {
                        new ProductPrice
                        {
                            Ppsid = "PP001",
                            Productid = "P001",
                            Price = 999.99m,
                            IsDefault = true
                        }
                    },
                    ProductCategories = new List<ProductCategory>
                    {
                        new ProductCategory
                        {
                            Id = "PC001",
                            Productid = "P001",
                            Categoryid = "C001"
                        }
                    }
                },
                new Product
                {
                    Productid = "P002",
                    ProductName = "Wireless Headphones",
                    Stockquantity = 100,
                    Bar = "HEAD001",
                    Sku = "SKU002",
                    Description = "Noise-cancelling wireless headphones",
                    Brand = "B002",
                    Unit = "U001",
                    IsActive = true,
                    IsDeleted = false,
                    StatusId = "PS001",
                    Metadata = "Audio",
                    CreatedAt = DateTime.UtcNow,
                    Dimensions = new List<Dimension>
                    {
                        new Dimension
                        {
                            Dimensionid = "D002",
                            Productid = "P002",
                            WeightValue = 0.3m,
                            HeightValue = 0.2m,
                            WidthValue = 0.15m,
                            LengthValue = 0.1m
                        }
                    },
                    ProductPrices = new List<ProductPrice>
                    {
                        new ProductPrice
                        {
                            Ppsid = "PP002",
                            Productid = "P002",
                            Price = 199.99m,
                            IsDefault = true
                        }
                    },
                    ProductCategories = new List<ProductCategory>
                    {
                        new ProductCategory
                        {
                            Id = "PC002",
                            Productid = "P002",
                            Categoryid = "C002"
                        }
                    }
                },
                new Product
                {
                    Productid = "P003",
                    ProductName = "Smartphone",
                    Stockquantity = 75,
                    Bar = "PHONE001",
                    Sku = "SKU003",
                    Description = "Latest smartphone with advanced camera",
                    Brand = "B003",
                    Unit = "U001",
                    IsActive = true,
                    IsDeleted = false,
                    StatusId = "PS001",
                    Metadata = "Mobile",
                    CreatedAt = DateTime.UtcNow,
                    Dimensions = new List<Dimension>
                    {
                        new Dimension
                        {
                            Dimensionid = "D003",
                            Productid = "P003",
                            WeightValue = 0.2m,
                            HeightValue = 0.15m,
                            WidthValue = 0.07m,
                            LengthValue = 0.15m
                        }
                    },
                    ProductPrices = new List<ProductPrice>
                    {
                        new ProductPrice
                        {
                            Ppsid = "PP003",
                            Productid = "P003",
                            Price = 799.99m,
                            IsDefault = true
                        }
                    },
                    ProductCategories = new List<ProductCategory>
                    {
                        new ProductCategory
                        {
                            Id = "PC003",
                            Productid = "P003",
                            Categoryid = "C003"
                        }
                    }
                }
            };

            return products;
        }

        public static List<Brand> GetBrands()
        {
            return new List<Brand>
            {
                new Brand
                {
                    Brand1 = "B001",
                    BrandName = "TechPro",
                    LogoData = "techpro_logo.png",
                    Website = "www.techpro.com",
                    ManufactureAddress = "123 Tech Street, Silicon Valley",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Brand
                {
                    Brand1 = "B002",
                    BrandName = "AudioMaster",
                    LogoData = "audiomaster_logo.png",
                    Website = "www.audiomaster.com",
                    ManufactureAddress = "456 Sound Ave, New York",
                    Country = "USA",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Brand
                {
                    Brand1 = "B003",
                    BrandName = "MobileTech",
                    LogoData = "mobiletech_logo.png",
                    Website = "www.mobiletech.com",
                    ManufactureAddress = "789 Mobile Blvd, Tokyo",
                    Country = "Japan",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<ProductStatus> GetProductStatuses()
        {
            return new List<ProductStatus>
            {
                new ProductStatus
                {
                    StatusId = "PS001",
                    Name = "Active",
                    Description = "Product is available for sale",
                    CreatedAt = DateTime.UtcNow
                },
                new ProductStatus
                {
                    StatusId = "PS002",
                    Name = "Out of Stock",
                    Description = "Product is temporarily unavailable",
                    CreatedAt = DateTime.UtcNow
                },
                new ProductStatus
                {
                    StatusId = "PS003",
                    Name = "Discontinued",
                    Description = "Product is no longer available",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<Unit> GetUnits()
        {
            return new List<Unit>
            {
                new Unit
                {
                    Unit1 = "U001",
                    UnitName = "Piece",
                    UnitSymbol = "pc",
                    CreatedAt = DateTime.UtcNow
                },
                new Unit
                {
                    Unit1 = "U002",
                    UnitName = "Kilogram",
                    UnitSymbol = "kg",
                    CreatedAt = DateTime.UtcNow
                },
                new Unit
                {
                    Unit1 = "U003",
                    UnitName = "Meter",
                    UnitSymbol = "m",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    Categoryid = "C001",
                    CategoryName = "Laptops",
                    Priority = 1,
                    Parentid = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Metadata = "Computers and laptops"
                },
                new Category
                {
                    Categoryid = "C002",
                    CategoryName = "Audio",
                    Priority = 2,
                    Parentid = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Metadata = "Audio equipment and accessories"
                },
                new Category
                {
                    Categoryid = "C003",
                    CategoryName = "Mobile",
                    Priority = 3,
                    Parentid = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Metadata = "Mobile phones and accessories"
                }
            };
        }
    }
} 