using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Images
{
    public class ImageDTO
    {
        public string Imageid { get; set; }
        public string? ImageData { get; set; }
        public string? Order { get; set; }
        public string? Publicid { get; set; }
    }
}
