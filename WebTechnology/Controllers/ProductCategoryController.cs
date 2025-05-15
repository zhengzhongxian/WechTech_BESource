﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.ProductCategories;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        /// <summary>
        /// Lấy danh sách danh mục của một sản phẩm
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các danh mục mà sản phẩm thuộc về.
        /// 
        /// Ví dụ: GET /api/product-categories/product/{productId}
        /// </remarks>
        /// <param name="productId">ID của sản phẩm cần lấy danh mục</param>
        /// <returns>Danh sách danh mục của sản phẩm</returns>
        /// <response code="200">Trả về danh sách danh mục</response>
        /// <response code="404">Không tìm thấy sản phẩm</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetCategoriesByProductId(string productId)
        {
            var response = await _productCategoryService.GetCategoriesByProductIdAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Thêm một danh mục cho sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép thêm một danh mục vào sản phẩm.
        /// 
        /// Cấu trúc dữ liệu gửi lên:
        /// - ProductId: ID của sản phẩm (bắt buộc)
        /// - CategoryId: ID của danh mục (bắt buộc)
        /// 
        /// Lưu ý:
        /// - Sản phẩm và danh mục phải tồn tại trong hệ thống
        /// - Mỗi sản phẩm chỉ có thể thuộc về một danh mục một lần
        /// 
        /// Ví dụ:
        /// ```json
        /// {
        ///   "productId": "123456",
        ///   "categoryId": "789012"
        /// }
        /// ```
        /// </remarks>
        /// <param name="createDto">Thông tin mối quan hệ sản phẩm-danh mục</param>
        /// <returns>Thông tin mối quan hệ đã tạo</returns>
        /// <response code="200">Thêm danh mục thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc mối quan hệ đã tồn tại</response>
        /// <response code="404">Không tìm thấy sản phẩm hoặc danh mục</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> AddProductCategory([FromBody] CreateProductCategoryDTO createDto)
        {
            var response = await _productCategoryService.AddProductCategoryAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa một danh mục khỏi sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép xóa mối quan hệ giữa sản phẩm và danh mục.
        /// 
        /// Lưu ý:
        /// - Cả sản phẩm và danh mục vẫn tồn tại sau khi xóa mối quan hệ
        /// - Chỉ xóa mối liên kết giữa chúng
        /// 
        /// Ví dụ: DELETE /api/product-categories/product/123456/category/789012
        /// </remarks>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>Kết quả xóa mối quan hệ</returns>
        /// <response code="200">Xóa mối quan hệ thành công</response>
        /// <response code="404">Không tìm thấy sản phẩm, danh mục hoặc mối quan hệ</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("product/{productId}/category/{categoryId}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> DeleteProductCategory(string productId, string categoryId)
        {
            var response = await _productCategoryService.DeleteProductCategoryAsync(productId, categoryId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
