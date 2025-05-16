﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách người dùng có vai trò là Admin và Staff có phân trang
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy danh sách người dùng có vai trò là Admin và Staff với các tùy chọn phân trang và tìm kiếm.
        /// 
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng bản ghi trên mỗi trang
        /// - SearchTerm: Từ khóa tìm kiếm (tìm theo username hoặc email)
        /// - SortBy: Sắp xếp theo trường nào (Username, Email, CreatedAt)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// - RoleFilter: Lọc theo vai trò (Admin, Staff hoặc để trống để lấy cả hai)
        /// </remarks>
        /// <param name="queryRequest">Thông tin phân trang và tìm kiếm</param>
        /// <returns>Danh sách người dùng có phân trang</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetPaginatedAdminStaffUsers([FromQuery] AdminStaffQueryRequest queryRequest)
        {
            var response = await _adminService.GetPaginatedAdminStaffUsersAsync(queryRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy thông tin chi tiết của một người dùng có vai trò là Admin hoặc Staff dựa trên ID.
        /// </remarks>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Thông tin chi tiết của người dùng</returns>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetAdminStaffDetail(string userId)
        {
            var response = await _adminService.GetAdminStaffDetailAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật thông tin của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin của một người dùng có vai trò là Admin hoặc Staff.
        /// Có thể cập nhật toàn bộ thông tin bao gồm cả mật khẩu.
        /// </remarks>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateAdminStaff(string userId, [FromBody] UpdateAdminStaffDTO updateDto)
        {
            var response = await _adminService.UpdateAdminStaffAsync(userId, updateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật toàn bộ thông tin của một khách hàng (bao gồm cả coupon và mật khẩu)
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật toàn bộ thông tin của một khách hàng, bao gồm cả coupon và mật khẩu.
        /// Chỉ Admin mới có quyền thực hiện API này.
        /// </remarks>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("customers/{customerId}")]
        public async Task<IActionResult> UpdateCustomerFull(string customerId, [FromBody] UpdateCustomerFullDTO updateDto)
        {
            var response = await _adminService.UpdateCustomerFullAsync(customerId, updateDto);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
