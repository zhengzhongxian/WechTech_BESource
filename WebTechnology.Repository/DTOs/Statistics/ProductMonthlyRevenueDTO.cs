using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Statistics
{
    /// <summary>
    /// DTO chứa thông tin doanh thu của một sản phẩm theo tháng
    /// </summary>
    public class ProductMonthlyRevenueDTO
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO chứa thông tin doanh thu của một sản phẩm theo năm
    /// </summary>
    public class ProductYearlyRevenueDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int Year { get; set; }
        public List<ProductMonthlyRevenueDTO> MonthlyRevenues { get; set; } = new List<ProductMonthlyRevenueDTO>();
        public decimal TotalRevenue { get; set; }
        public int TotalQuantity { get; set; }
    }
}
