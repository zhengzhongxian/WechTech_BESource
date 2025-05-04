using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Images
{
    public class CreateImageDTO
    {
        [Required(ErrorMessage = "ProductId là bắt buộc")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; }

        public string? Order { get; set; }
    }
}
