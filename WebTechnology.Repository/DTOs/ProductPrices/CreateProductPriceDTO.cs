using System;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.ProductPrices
{
    public class ProductPriceCreateDTO
    {
        public string? ProductId { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải là số dương")]
        public decimal Price { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
