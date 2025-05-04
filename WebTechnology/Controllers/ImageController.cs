using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(
            IImageService imageService,
            ILogger<ImageController> logger)
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
        /// Cập nhật thứ tự hiển thị của hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <param name="order">Thứ tự mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}/order")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] string order)
        {
            var response = await _imageService.UpdateOrderAsync(id, order);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <returns>Kết quả xóa hình ảnh</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            var response = await _imageService.DeleteImageAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
