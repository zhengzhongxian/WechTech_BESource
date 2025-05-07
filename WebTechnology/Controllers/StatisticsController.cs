using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Lấy doanh thu theo từng tháng trong một năm
        /// </summary>
        /// <param name="year">Năm cần lấy doanh thu (mặc định là năm hiện tại)</param>
        /// <returns>Doanh thu theo từng tháng trong năm</returns>
        [HttpGet("monthly-revenue")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int? year = null)
        {
            // Nếu không có năm được chỉ định, sử dụng năm hiện tại
            int requestedYear = year ?? DateTime.Now.Year;

            string token = Request.Headers["Authorization"].ToString();
            var response = await _statisticsService.GetMonthlyRevenueForYearAsync(requestedYear, token);

            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy doanh số sản phẩm theo tháng và năm
        /// </summary>
        /// <param name="month">Tháng cần lấy doanh số (1-12)</param>
        /// <param name="year">Năm cần lấy doanh số</param>
        /// <returns>Danh sách sản phẩm và doanh số trong tháng</returns>
        [HttpGet("product-sales/{year}/{month}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetProductSalesForMonth(int year, int month)
        {
            try
            {
                // Lấy token từ header
                string token = Request.Headers["Authorization"].ToString();

                // Gọi service để lấy doanh số sản phẩm theo tháng
                var response = await _statisticsService.GetProductSalesForMonthAsync(month, year, token);

                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
