using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStatusController : ControllerBase
    {
        private readonly IProductStatusService _productStatusService;
        public ProductStatusController(IProductStatusService productStatusService)
        {
            _productStatusService = productStatusService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductStatus()
        {
            var result = await _productStatusService.GetAllProductStatus();
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
