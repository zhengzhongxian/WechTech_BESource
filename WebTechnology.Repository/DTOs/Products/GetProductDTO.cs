using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.DTOs.Images;

namespace WebTechnology.Repository.DTOs.Products
{
    public class GetProductDTO
    {
        public string Productid { get; set; } = null!;

        public string? ProductName { get; set; }

        public int? Stockquantity { get; set; }

        public string? Bar { get; set; }

        public string? Sku { get; set; }

        public string? Description { get; set; }

        public string? Brand { get; set; }

        public string? Unit { get; set; }
        public string? StatusName { get; set; } //trong bảng ProductStatus có property:Name
        public decimal? PriceActive { get; set; } //trong bảng ProductPrice có property: PriceActive (chỉ cần lấy 1 cái đầu tiên)
        public decimal? PriceDefault { get; set; } //trong bảng ProductPrice có property: PriceDefault (chỉ cần lấy 1 cái đầu tiên)
        public List<ImageDTO> imageDTOs { get; set; } = new List<ImageDTO>();
        public List<DimensionDTO> Dimensions { get; set; } = new List<DimensionDTO>();
    }
}
