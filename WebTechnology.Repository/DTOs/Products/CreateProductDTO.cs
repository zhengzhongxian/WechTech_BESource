using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Products
{
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Số lượng trong kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng trong kho phải là số dương")]
        public int StockQuantity { get; set; }

        public string? Bar { get; set; }

        public string? Sku { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        public string? Brand { get; set; }

        public string? Unit { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public string? StatusId { get; set; }

        public string? Metadata { get; set; }

        // For dimensions
        public decimal? WeightValue { get; set; }
        public decimal? HeightValue { get; set; }
        public decimal? WidthValue { get; set; }
        public decimal? LengthValue { get; set; }

        // For price
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải là số dương")]
        public decimal Price { get; set; }

        // For categories
        public List<string>? CategoryIds { get; set; } = new List<string>();

        // For images
        public List<string>? ImageData { get; set; } = new List<string>();
    }
}