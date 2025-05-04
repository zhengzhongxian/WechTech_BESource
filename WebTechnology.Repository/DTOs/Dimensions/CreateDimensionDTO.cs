using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Dimensions
{
    public class CreateDimensionDTO
    {
        public string? ProductId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Trọng lượng phải là số dương")]
        public decimal? WeightValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chiều cao phải là số dương")]
        public decimal? HeightValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chiều rộng phải là số dương")]
        public decimal? WidthValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chiều dài phải là số dương")]
        public decimal? LengthValue { get; set; }

        public string? Metadata { get; set; }
    }
}