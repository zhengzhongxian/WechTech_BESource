using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Trends
{
    public class CreateTrendDTO
    {
        [Required(ErrorMessage = "Tên xu hướng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên xu hướng không được vượt quá 100 ký tự")]
        public string TrendName { get; set; }

        public string? BannerData { get; set; }

        [Required(ErrorMessage = "Độ ưu tiên là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Độ ưu tiên phải từ 1 đến 10")]
        //[DefaultValue(null)]
        public int Priority { get; set; }
    }
}
