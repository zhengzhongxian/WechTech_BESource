using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.DTOs.Images;

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

        public List<CreateProductPriceDTO> ProductPrices { get; set; } = new List<CreateProductPriceDTO>();
        public List<CreateImageDTONew> Images { get; set; } = new List<CreateImageDTONew>();
        public List<CreateDimensionDTONew> Dimensions { get; set; } = new List<CreateDimensionDTONew>();
    }
}