using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WebTechnology.Repository.DTOs.Trends;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrendController : ControllerBase
    {
        private readonly ITrendService _trendService;
        private readonly ILogger<TrendController> _logger;
        public TrendController(ITrendService trendService, ILogger<TrendController> logger)
        {
            _trendService = trendService;
            _logger = logger;
        }

        /// <summary>
        /// Tạo mới một xu hướng
        /// <param name="createDto">Dữ liệu tạo xu hướng</param>
        /// <returns>Trả về trạng thái kết quả</returns>
        ///  </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTrend([FromBody] CreateTrendDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Trend>.ErrorResponse(
                    "Dữ liệu không hợp lệ",
                    HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _trendService.CreateTrendAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách xu hướng
        /// </summary>
        /// <returns>Danh sách xu hướng</returns>
        [HttpGet]
        public async Task<IActionResult> GetTrend()
        {
            var response = await _trendService.GetTrendsAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật một phần xu hướng bằng PATCH
        /// <returns>Xu hướng đã được cập nhật</returns>
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTrend(string id, [FromBody] JsonPatchDocument<Trend> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("Patch document không hợp lệ");

            var response = await _trendService.PatchTrendAsync(id, patchDoc);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrend(string id)
        {
            var response = await _trendService.DeleteTrendAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
