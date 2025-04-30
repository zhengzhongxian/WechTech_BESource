using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetOrderByIdAsync(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetAllOrdersAsync(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get orders by current user
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetOrdersByUserIdAsync(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO orderRequest)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.CreateOrderAsync(orderRequest, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderRequestDTO orderRequest)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.UpdateOrderAsync(id, orderRequest, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.DeleteOrderAsync(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        [HttpPut("{orderId}/status/{statusId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string statusId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.UpdateOrderStatusAsync(orderId, statusId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Calculate order total
        /// </summary>
        [HttpGet("{orderId}/total")]
        public async Task<IActionResult> CalculateOrderTotal(string orderId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.CalculateOrderTotalAsync(orderId, token);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
