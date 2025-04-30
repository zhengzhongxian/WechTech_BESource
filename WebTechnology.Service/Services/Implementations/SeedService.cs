using Microsoft.EntityFrameworkCore;
using WebTechnology.API;
using WebTechnology.Repository.SeedData;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class SeedService : ISeedService
    {
        private readonly WebTech _context;

        public SeedService(WebTech context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            // Seed Brands
            if (!await _context.Brands.AnyAsync())
            {
                await _context.Brands.AddRangeAsync(ProductSeedData.GetBrands());
                await _context.SaveChangesAsync();
            }

            // Seed Units
            if (!await _context.Units.AnyAsync())
            {
                await _context.Units.AddRangeAsync(ProductSeedData.GetUnits());
                await _context.SaveChangesAsync();
            }

            // Seed Product Statuses
            if (!await _context.ProductStatuses.AnyAsync())
            {
                await _context.ProductStatuses.AddRangeAsync(ProductSeedData.GetProductStatuses());
                await _context.SaveChangesAsync();
            }

            // Seed Categories
            if (!await _context.Categories.AnyAsync())
            {
                await _context.Categories.AddRangeAsync(ProductSeedData.GetCategories());
                await _context.SaveChangesAsync();
            }

            // Seed Products
            if (!await _context.Products.AnyAsync())
            {
                var products = ProductSeedData.GetProducts();
                foreach (var product in products)
                {
                    // Check if brand exists
                    if (!await _context.Brands.AnyAsync(b => b.Brand1 == product.Brand))
                    {
                        throw new Exception($"Brand {product.Brand} not found");
                    }

                    // Check if unit exists
                    if (!await _context.Units.AnyAsync(u => u.Unit1 == product.Unit))
                    {
                        throw new Exception($"Unit {product.Unit} not found");
                    }

                    // Check if status exists
                    if (!await _context.ProductStatuses.AnyAsync(s => s.StatusId == product.StatusId))
                    {
                        throw new Exception($"Status {product.StatusId} not found");
                    }

                    // Check if all categories exist
                    foreach (var category in product.ProductCategories)
                    {
                        if (!await _context.Categories.AnyAsync(c => c.Categoryid == category.Categoryid))
                        {
                            throw new Exception($"Category {category.Categoryid} not found");
                        }
                    }
                }

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
        }
    }
} 