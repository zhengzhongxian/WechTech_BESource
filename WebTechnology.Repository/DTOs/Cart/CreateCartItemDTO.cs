using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Cart
{
    public class CreateCartItemDTO
    {
        public string Id = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Không được trống ProductId")]
        public string ProductId { get; set; }
        [Required(ErrorMessage = "Không được trống Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
