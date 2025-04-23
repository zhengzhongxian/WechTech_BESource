using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Products
{
    public class GetProductToCart
    {
        public string ProductImgData { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPriceIsDefault { get; set; }
        public decimal ProductPriceIsActive { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
