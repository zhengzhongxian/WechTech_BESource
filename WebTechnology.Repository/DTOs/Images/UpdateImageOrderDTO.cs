using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Images
{
    public class UpdateImageOrderDTO
    {
        [Required(ErrorMessage = "Order là bắt buộc")]
        public string Order { get; set; }
    }
}
