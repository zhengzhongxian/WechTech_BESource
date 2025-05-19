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

        /// <summary>
        /// Lấy số lượng khách hàng đang online
        /// </summary>
        /// <returns>Số lượng khách hàng đang online</returns>
        [HttpGet("online-customers-count")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetOnlineCustomersCount()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString();
                var response = await _statisticsService.GetOnlineCustomersCountAsync(token);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy doanh thu trong ngày hôm nay
        /// </summary>
        /// <returns>Doanh thu trong ngày hôm nay</returns>
        [HttpGet("today-revenue")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString();
                var response = await _statisticsService.GetTodayRevenueAsync(token);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy số lượng sản phẩm bán được trong ngày hôm nay
        /// </summary>
        /// <returns>Số lượng sản phẩm bán được trong ngày hôm nay</returns>
        [HttpGet("today-sold-products-count")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetTodaySoldProductsCount()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString();
                var response = await _statisticsService.GetTodaySoldProductsCountAsync(token);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy số lượng đơn hàng đang chờ xử lý (PENDING)
        /// </summary>
        /// <returns>Số lượng đơn hàng đang chờ xử lý</returns>
        [HttpGet("pending-orders-count")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetPendingOrdersCount()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString();
                var response = await _statisticsService.GetPendingOrdersCountAsync(token);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy doanh thu của một sản phẩm theo từng tháng trong năm
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="year">Năm cần lấy doanh thu</param>
        /// <returns>Thông tin doanh thu của sản phẩm theo từng tháng</returns>
        [HttpGet("product-monthly-revenue/{productId}/{year}")]
        [Authorize(Policy = "AdminOnly")] // Chỉ Admin mới được xem
        public async Task<IActionResult> GetProductMonthlyRevenueForYear(string productId, int year)
        {
            try
            {
                // Lấy token từ header
                string token = Request.Headers["Authorization"].ToString();

                // Gọi service để lấy doanh thu sản phẩm theo tháng
                var response = await _statisticsService.GetProductMonthlyRevenueForYearAsync(productId, year, token);

                return StatusCode((int)response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
