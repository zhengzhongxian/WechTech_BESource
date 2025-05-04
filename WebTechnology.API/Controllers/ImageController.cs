using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.Models.Entities;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        /// <summary>
        /// Thêm hình ảnh mới cho sản phẩm
        /// </summary>
        /// <param name="createDto">Dữ liệu hình ảnh</param>
        /// <returns>Kết quả thêm hình ảnh</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddImage([FromBody] CreateImageDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Image>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _imageService.AddImageAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách hình ảnh của sản phẩm
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <returns>Danh sách hình ảnh</returns>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetImagesByProductId(string productId)
        {
            var response = await _imageService.GetImagesByProductIdAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <param name="updateDto">Dữ liệu cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateImage(string id, [FromBody] UpdateImageDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Image>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _imageService.UpdateImageAsync(id, updateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật thứ tự hiển thị của hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <param name="updateDto">Dữ liệu cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}/order")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateImageOrder(string id, [FromBody] UpdateImageOrderDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Image>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _imageService.UpdateImageOrderAsync(id, updateDto.Order);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <returns>Kết quả xóa hình ảnh</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            var response = await _imageService.DeleteImageAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
