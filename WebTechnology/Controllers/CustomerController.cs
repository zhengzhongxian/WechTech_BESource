using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Services.Interfaces;
using WebTechnology.Service.Models;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        /// <summary>
        /// lấy thông tin user (lưu ý: trước đó đăng ký thì chỉ có trường avt là được null nếu ko thì lấy ra sẽ lỗi)
        /// </summary>
        [HttpGet("info")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetUserInfo()
        {
            var token = Request.Headers["Authorization"].ToString();
            var response = await _customerService.GetCustomerInfo(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// update này thì ko có update email nhé (thậm chí ko nên làm tính năng update email)
        /// </summary>
        [HttpPatch("update")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> UpdateUserInfo(JsonPatchDocument<Customer> patchDoc)
        {
            var token = Request.Headers["Authorization"].ToString();
            var response = await _customerService.UpdateCustomerInfo(token, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách người dùng có phân trang (chỉ dành cho Admin)
        /// </summary>
        [HttpGet("users")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPaginatedUsers([FromQuery] UserQueryRequest queryRequest)
        {
            var response = await _customerService.GetPaginatedUsersAsync(queryRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("profile")]
        [Authorize(Policy = "CustomerOnly")]
        public IActionResult GetProfile()
        {
            return Ok(new { Message = "Thông tin cá nhân của khách hàng" });
        }

        [HttpGet("orders")]
        [Authorize(Policy = "CustomerOnly")]
        public IActionResult GetOrders()
        {
            return Ok(new { Message = "Danh sách đơn hàng của khách hàng" });
        }

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        [HttpGet("{customerId}/detail")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetCustomerDetail(string customerId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _customerService.GetCustomerDetailAsync(customerId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        [HttpPut("{customerId}/detail")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCustomerDetail(string customerId, [FromBody] UpdateCustomerDetailDTO updateDto)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _customerService.UpdateCustomerDetailAsync(customerId, updateDto, token);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}