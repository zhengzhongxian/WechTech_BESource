using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Statistics
{
    public class ProductSalesDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class MonthlySalesDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public List<ProductSalesDTO> Products { get; set; } = new List<ProductSalesDTO>();
        public decimal TotalSales { get; set; }
        public int TotalQuantity { get; set; }
    }
}
