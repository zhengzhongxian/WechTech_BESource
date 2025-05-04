using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Images
{
    public class UpdateImageDTO
    {
        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; } // Base64 encoded image
    }
}
