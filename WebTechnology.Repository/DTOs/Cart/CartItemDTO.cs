using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Products;

namespace WebTechnology.Repository.DTOs.Cart
{
    public class CartItemDTO
    {
        public string Id { get; set; }
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public GetProductToCart GetProductToCart { get; set; }
        public int Quantity { get; set; }
    }
}
