using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPriceController : ControllerBase
    {
        private readonly IProductPriceService _productPriceService;
        public ProductPriceController(IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductPrices(string productId)
        {
            var result = await _productPriceService.GetProductPricesAsync(productId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
