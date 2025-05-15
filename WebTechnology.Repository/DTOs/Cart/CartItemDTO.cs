using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Products;

namespace WebTechnology.Repository.DTOs.Cart
{
    /// <summary>
    /// DTO cho thông tin sản phẩm trong giỏ hàng
    /// </summary>
    public class CartItemDTO
    {
        /// <summary>
        /// ID của mục trong giỏ hàng
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ID của giỏ hàng
        /// </summary>
        public string CartId { get; set; }

        /// <summary>
        /// ID của sản phẩm
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Thông tin sản phẩm hiển thị trong giỏ hàng
        /// </summary>
        public GetProductToCart GetProductToCart { get; set; }

        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Kiểm tra sản phẩm còn tồn tại trong hệ thống hay không
        /// </summary>
        public bool IsProductExists { get; set; } = true;

        /// <summary>
        /// Kiểm tra số lượng sản phẩm có vượt quá tồn kho hay không
        /// </summary>
        public bool IsStockAvailable { get; set; } = true;

        /// <summary>
        /// Kiểm tra sản phẩm đã hết hàng hay chưa
        /// </summary>
        public bool IsOutOfStock { get; set; } = false;

        /// <summary>
        /// Số lượng tồn kho hiện tại của sản phẩm
        /// </summary>
        public int? AvailableStock { get; set; }
    }
}
