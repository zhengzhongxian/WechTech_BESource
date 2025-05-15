using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Parents;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly ILogger<ParentController> _logger;

        public ParentController(IParentService parentService, ILogger<ParentController> logger)
        {
            _parentService = parentService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục cha
        /// </summary>
        /// <returns>Danh sách danh mục cha</returns>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<ParentDTO>>>> GetAllParents()
        {
            try
            {
                var response = await _parentService.GetAllParentsAsync();
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục cha");
                return StatusCode(500, new ServiceResponse<IEnumerable<ParentDTO>>
                {
                    Message = "Lỗi server khi lấy danh sách danh mục cha",
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Lấy thông tin danh mục cha theo ID
        /// </summary>
        /// <param name="id">ID của danh mục cha</param>
        /// <returns>Thông tin danh mục cha</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ParentDTO>>> GetParentById(string id)
        {
            try
            {
                var response = await _parentService.GetParentByIdAsync(id);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin danh mục cha với ID: {Id}", id);
                return StatusCode(500, new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi server khi lấy thông tin danh mục cha",
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Tạo mới danh mục cha
        /// </summary>
        /// <param name="createParentDTO">Thông tin danh mục cha cần tạo</param>
        /// <returns>Thông tin danh mục cha đã tạo</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceResponse<ParentDTO>>> CreateParent([FromBody] CreateParentDTO createParentDTO)
        {
            try
            {
                var response = await _parentService.CreateParentAsync(createParentDTO);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục cha");
                return StatusCode(500, new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi server khi tạo danh mục cha",
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin danh mục cha
        /// </summary>
        /// <param name="id">ID của danh mục cha</param>
        /// <param name="updateParentDTO">Thông tin cập nhật</param>
        /// <returns>Thông tin danh mục cha đã cập nhật</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceResponse<ParentDTO>>> UpdateParent(string id, [FromBody] CreateParentDTO updateParentDTO)
        {
            try
            {
                var response = await _parentService.UpdateParentAsync(id, updateParentDTO);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật danh mục cha với ID: {Id}", id);
                return StatusCode(500, new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi server khi cập nhật danh mục cha",
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Xóa danh mục cha
        /// </summary>
        /// <param name="id">ID của danh mục cha</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteParent(string id)
        {
            try
            {
                var response = await _parentService.DeleteParentAsync(id);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa danh mục cha với ID: {Id}", id);
                return StatusCode(500, new ServiceResponse<bool>
                {
                    Message = "Lỗi server khi xóa danh mục cha",
                    Success = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
