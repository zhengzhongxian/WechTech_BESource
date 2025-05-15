
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        /// <summary>
        /// sortBy: "ProductName", "Price" nếu cần thêm thì liên hệ BE <br/>
        /// sortAcending: true/false (true: isdefault)
        /// </summary>
        [HttpGet("get-products")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryRequest request)
        {
            var response = await _productService.GetProductsWithDetailsAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("product-trends-by-productid")]
        public async Task<IActionResult> GetListTrendsByProductId(string productId)
        {
            var response = await _productService.GetListTrendsByProductId(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("delete-product-trends")]
        public async Task<IActionResult> DeleteProductTrends(string id)
        {
            var response = await _productService.DeleteProductTrendsAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Tạo mới một sản phẩm
        /// </summary>
        /// <param name="createDto">Dữ liệu tạo sản phẩm</param>
        /// <returns>Trả về trạng thái kết quả</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Product>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _productService.CreateProductAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        /// <returns>Danh sách sản phẩm</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetProductsAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <returns>Thông tin sản phẩm</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật một phần sản phẩm bằng PATCH
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <param name="patchDoc">Document PATCH</param>
        /// <returns>Sản phẩm đã được cập nhật</returns>
        [HttpPatch("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> PatchProduct(string id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest(ServiceResponse<Product>.ErrorResponse(
                    "Patch document không hợp lệ",
                    HttpStatusCode.BadRequest));

            var response = await _productService.PatchProductAsync(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <returns>Kết quả xóa sản phẩm</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
