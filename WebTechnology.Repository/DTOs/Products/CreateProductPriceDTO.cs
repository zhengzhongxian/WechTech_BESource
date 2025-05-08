using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Products
{
    public class CreateProductPriceDTO
    {
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
