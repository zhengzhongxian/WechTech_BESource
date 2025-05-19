using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Service.Services.Interfaces;
using System.Security.Claims;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayosController : ControllerBase
    {
        private readonly IPayosService _payosService;
        private readonly ILogger<PayosController> _logger;
        private readonly ITokenService _tokenService;

        public PayosController(
            IPayosService payosService,
            ILogger<PayosController> logger,
            ITokenService tokenService)
        {
            _payosService = payosService;
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Tạo link thanh toán Payos
        /// </summary>
        /// <remarks>
        /// API này tạo một link thanh toán qua cổng Payos.
        /// Thông tin khách hàng sẽ được lấy từ token, không cần truyền vào.
        ///
        /// **Quyền truy cập:**
        /// - Khách hàng đã đăng nhập
        ///
        /// **Cấu trúc dữ liệu trả về:**
        /// - **paymentLinkId**: ID giao dịch trong hệ thống Payos
        /// - **checkoutUrl**: URL thanh toán
        /// - **qrCode**: Mã QR thanh toán
        /// - **expiredAt**: Thời gian hết hạn
        /// </remarks>
        /// <param name="request">Thông tin thanh toán cơ bản (orderId, returnUrl, cancelUrl)</param>
        /// <returns>Thông tin link thanh toán</returns>
        /// <response code="200">Trả về thông tin link thanh toán</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPost("create-payment")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreatePayment([FromBody] PayosCreatePaymentLinkRequest request)
        {
            _logger.LogInformation("Request received to create Payos payment link");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy thông tin từ token
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var email = _tokenService.GetEmailFromToken(token);
            var name = _tokenService.GetNameFromToken(token);

            // Thêm thông tin khách hàng từ token
            if (request.CustomerInfo == null)
            {
                request.CustomerInfo = new PayosCustomerInfo();
            }

            request.CustomerInfo.Email = email;
            request.CustomerInfo.Name = name;

            var response = await _payosService.CreatePaymentLinkAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Webhook từ Payos
        /// </summary>
        /// <remarks>
        /// API này nhận webhook từ Payos khi trạng thái thanh toán thay đổi.
        ///
        /// **Quyền truy cập:**
        /// - Chỉ Payos có thể gọi API này
        /// </remarks>
        /// <param name="webhookRequest">Dữ liệu webhook</param>
        /// <returns>Kết quả xử lý</returns>
        /// <response code="200">Xử lý webhook thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessWebhook([FromBody] PayosWebhookRequest webhookRequest)
        {
            _logger.LogInformation("Received Payos webhook");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _payosService.ProcessWebhookAsync(webhookRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán
        /// </summary>
        /// <remarks>
        /// API này kiểm tra trạng thái thanh toán của một giao dịch Payos.
        ///
        /// **Quyền truy cập:**
        /// - Khách hàng đã đăng nhập
        /// </remarks>
        /// <param name="paymentLinkId">ID giao dịch trong hệ thống Payos</param>
        /// <returns>Thông tin trạng thái thanh toán</returns>
        /// <response code="200">Trả về thông tin trạng thái thanh toán</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("check-status/{paymentLinkId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CheckPaymentStatus(string paymentLinkId)
        {
            _logger.LogInformation("Request received to check Payos payment status");

            if (string.IsNullOrEmpty(paymentLinkId))
            {
                return BadRequest("Payment link ID is required");
            }

            var response = await _payosService.CheckPaymentStatusAsync(paymentLinkId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán và cập nhật đơn hàng (callback)
        /// </summary>
        /// <remarks>
        /// API này được gọi từ trang callback sau khi thanh toán.
        /// Nó sẽ kiểm tra trạng thái thanh toán và cập nhật trạng thái đơn hàng nếu thanh toán thành công.
        ///
        /// **Quyền truy cập:**
        /// - Không yêu cầu xác thực
        /// </remarks>
        /// <param name="paymentLinkId">ID giao dịch trong hệ thống Payos</param>
        /// <returns>Thông tin trạng thái thanh toán</returns>
        /// <response code="200">Trả về thông tin trạng thái thanh toán</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("check-status-callback/{paymentLinkId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPaymentStatusCallback(string paymentLinkId)
        {
            _logger.LogInformation("Callback received to check Payos payment status");

            if (string.IsNullOrEmpty(paymentLinkId))
            {
                return BadRequest("Payment link ID is required");
            }

            var response = await _payosService.CheckPaymentStatusAsync(paymentLinkId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
