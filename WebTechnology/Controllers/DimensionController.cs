using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionController : ControllerBase
    {
        private readonly IDimensionService _dimensionService;
        public DimensionController(IDimensionService dimensionService)
        {
            _dimensionService = dimensionService;
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetDimensions(string productId)
        {
            var response = await _dimensionService.GetDimensionAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
