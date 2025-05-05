using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

        /// <summary>
        /// Get all order statuses
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrderStatuses()
        {
            var response = await _orderStatusService.GetAllOrderStatusAsync();
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
