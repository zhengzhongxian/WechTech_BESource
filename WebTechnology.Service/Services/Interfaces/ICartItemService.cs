using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Cart;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ quản lý giỏ hàng
    /// </summary>
    public interface ICartItemService
    {
        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <param name="cartItem">Thông tin sản phẩm cần thêm</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả thêm sản phẩm vào giỏ hàng</returns>
        Task<ServiceResponse<string>> AddToCart(CreateCartItemDTO cartItem, string token);

        /// <summary>
        /// Lấy danh sách sản phẩm trong giỏ hàng có phân trang
        /// </summary>
        /// <param name="token">Token xác thực</param>
        /// <param name="request">Thông tin phân trang</param>
        /// <returns>Danh sách sản phẩm trong giỏ hàng có phân trang</returns>
        Task<ServiceResponse<PaginatedResult<CartItemDTO>>> GetListCartItem(string token, CartItemQueryRequest request);

        /// <summary>
        /// Cập nhật sản phẩm trong giỏ hàng
        /// </summary>
        /// <param name="id">ID của sản phẩm trong giỏ hàng</param>
        /// <param name="patchDoc">Thông tin cập nhật</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả cập nhật sản phẩm trong giỏ hàng</returns>
        Task<ServiceResponse<string>> UpdateCartItem(string id, JsonPatchDocument<CartItem> patchDoc, string token);

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        /// <param name="id">ID của sản phẩm trong giỏ hàng</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả xóa sản phẩm khỏi giỏ hàng</returns>
        Task<ServiceResponse<string>> DeleteCartItem(string id, string token);
    }
}
