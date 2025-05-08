using System;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Images
{
    public class CreateImageDTONew
    {
        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; }

        public string? Order { get; set; }
    }
}
