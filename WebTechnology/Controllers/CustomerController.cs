using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;
using WebTechnology.Service.Models;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
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
        public async Task<IActionResult> UpdateUserInfo(JsonPatchDocument<Customer> patchDoc)
        {
            var token = Request.Headers["Authorization"].ToString();
            var response = await _customerService.UpdateCustomerInfo(token, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok(new { Message = "Thông tin cá nhân của khách hàng" });
        }

        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            return Ok(new { Message = "Danh sách đơn hàng của khách hàng" });
        }
    }
}