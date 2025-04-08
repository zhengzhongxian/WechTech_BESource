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
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryRequest request)
        {
            var response = await _productService.GetProductsWithDetailsAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
