using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả phương thức thanh toán
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả các phương thức thanh toán có sẵn trong hệ thống.
        /// 
        /// **Quyền truy cập:**
        /// - Tất cả người dùng đều có thể truy cập API này
        /// 
        /// **Cấu trúc dữ liệu trả về:**
        /// - **paymentId**: ID của phương thức thanh toán
        /// - **paymentName**: Tên phương thức thanh toán
        /// - **description**: Mô tả về phương thức thanh toán
        /// </remarks>
        /// <returns>Danh sách các phương thức thanh toán</returns>
        /// <response code="200">Trả về danh sách phương thức thanh toán</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            _logger.LogInformation("Request received to get all payment methods");
            
            var response = await _paymentService.GetAllPaymentsAsync();
            
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
