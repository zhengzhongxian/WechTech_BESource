using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
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
    }
}
