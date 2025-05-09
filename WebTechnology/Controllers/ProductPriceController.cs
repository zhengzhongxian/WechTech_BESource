using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.ProductPrices;
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

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductPrices(string productId)
        {
            var response = await _productPriceService.GetProductPricesAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{priceId}")]
        public async Task<IActionResult> GetProductPriceById(string priceId)
        {
            var response = await _productPriceService.GetProductPriceByIdAsync(priceId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductPrice([FromBody] ProductPriceCreateDTO createDto)
        {
            var response = await _productPriceService.CreateProductPriceAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{priceId}")]
        public async Task<IActionResult> UpdateProductPrice(string priceId, [FromBody] JsonPatchDocument<ProductPrice> patchDoc)
        {
            var response = await _productPriceService.UpdateProductPriceAsync(priceId, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{priceId}")]
        public async Task<IActionResult> DeleteProductPrice(string priceId)
        {
            var response = await _productPriceService.DeleteProductPriceAsync(priceId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{priceId}/set-default")]
        public async Task<IActionResult> SetDefaultPrice(string priceId, [FromBody] SetDefaultPriceRequest request)
        {
            var response = await _productPriceService.SetDefaultPriceAsync(priceId, request.IsDefault);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{priceId}/set-status")]
        public async Task<IActionResult> SetPriceStatus(string priceId, [FromBody] SetPriceStatusRequest request)
        {
            var response = await _productPriceService.SetPriceStatusAsync(priceId, request.IsActive);
            return StatusCode((int)response.StatusCode, response);
        }
    }

    public class SetDefaultPriceRequest
    {
        public bool IsDefault { get; set; }
    }

    public class SetPriceStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
