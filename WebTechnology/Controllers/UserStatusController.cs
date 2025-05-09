using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.UserStatuses;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatusController : ControllerBase
    {
        private readonly IUserStatusService _userStatusService;
        private readonly ILogger<UserStatusController> _logger;

        public UserStatusController(IUserStatusService userStatusService, ILogger<UserStatusController> logger)
        {
            _userStatusService = userStatusService;
            _logger = logger;
        }

        /// <summary>
        /// Tạo mới một trạng thái người dùng
        /// </summary>
        /// <param name="createDto">Dữ liệu tạo trạng thái người dùng</param>
        /// <returns>Trả về trạng thái kết quả</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUserStatus([FromBody] CreateUserStatusDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<UserStatus>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _userStatusService.CreateUserStatusAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách trạng thái người dùng
        /// </summary>
        /// <returns>Danh sách trạng thái người dùng</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserStatuses()
        {
            var response = await _userStatusService.GetUserStatusesAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật một phần trạng thái người dùng bằng PATCH
        /// </summary>
        /// <param name="id">ID của trạng thái người dùng</param>
        /// <param name="patchDoc">Tài liệu JSON Patch</param>
        /// <returns>Trạng thái người dùng đã được cập nhật</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUserStatus(string id, [FromBody] JsonPatchDocument<UserStatus> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("Patch document không hợp lệ");

            var response = await _userStatusService.PatchUserStatusAsync(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa trạng thái người dùng
        /// </summary>
        /// <param name="id">ID của trạng thái người dùng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserStatus(string id)
        {
            var response = await _userStatusService.DeleteUserStatusAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}