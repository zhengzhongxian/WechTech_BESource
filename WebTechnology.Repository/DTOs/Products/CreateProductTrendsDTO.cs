using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Products
{
    public class CreateProductTrendsDTO
    {
        [Required(ErrorMessage = "productId là bát buộc")]
        public string Productid { get; set; }

        [Required(ErrorMessage = "trend là bát buộc")]
        public string Trend { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
