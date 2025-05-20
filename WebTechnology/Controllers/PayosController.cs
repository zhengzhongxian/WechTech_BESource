using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayosController : ControllerBase
    {
        private readonly IPayosService _payosService;
        private readonly ILogger<PayosController> _logger;

        public PayosController(IPayosService payosService, ILogger<PayosController> logger)
        {
            _payosService = payosService;
            _logger = logger;
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
            try
            {
                _logger.LogInformation("Creating Payos payment for order {OrderId}", request.OrderId);

                // Lấy thông tin khách hàng từ token
                var customerEmail = User.FindFirstValue(ClaimTypes.Email);
                var customerName = User.FindFirstValue(ClaimTypes.Name);
                var customerPhone = User.FindFirstValue(ClaimTypes.MobilePhone);

                // Thêm thông tin khách hàng vào request
                if (!string.IsNullOrEmpty(customerEmail) && !string.IsNullOrEmpty(customerName))
                {
                    request.CustomerInfo = new PayosCustomerInfo
                    {
                        Email = customerEmail,
                        Name = customerName,
                        Phone = customerPhone ?? ""
                    };
                }

                var result = await _payosService.CreatePaymentLinkAsync(request);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Payos payment");
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo link thanh toán" });
            }
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
            try
            {
                _logger.LogInformation("Received Payos webhook");

                var result = await _payosService.ProcessWebhookAsync(webhookRequest);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Payos webhook");
                return StatusCode(500, new { success = false, message = "Lỗi khi xử lý webhook" });
            }
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
            try
            {
                _logger.LogInformation("Checking Payos payment status for {PaymentLinkId}", paymentLinkId);

                var result = await _payosService.CheckPaymentStatusAsync(paymentLinkId);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Payos payment status");
                return StatusCode(500, new { success = false, message = "Lỗi khi kiểm tra trạng thái thanh toán" });
            }
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
        [HttpGet("callback/{paymentLinkId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPaymentStatusCallback(string paymentLinkId)
        {
            try
            {
                _logger.LogInformation("Callback for Payos payment {PaymentLinkId}", paymentLinkId);

                var result = await _payosService.CheckPaymentStatusAsync(paymentLinkId);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Payos callback");
                return StatusCode(500, new { success = false, message = "Lỗi khi xử lý callback" });
            }
        }

        /// <summary>
        /// Endpoint test cho webhook Payos
        /// </summary>
        /// <remarks>
        /// API này dùng để kiểm tra xem webhook có thể truy cập được không.
        /// </remarks>
        /// <returns>Thông báo thành công</returns>
        [HttpGet("test-webhook")]
        [AllowAnonymous]
        public IActionResult TestWebhook()
        {
            return Ok(new { success = true, message = "Webhook endpoint is accessible" });
        }

        /// <summary>
        /// Xác nhận webhook URL với Payos
        /// </summary>
        /// <remarks>
        /// API này xác nhận webhook URL với Payos.
        /// 
        /// **Quyền truy cập:**
        /// - Admin
        /// </remarks>
        /// <param name="webhookUrl">URL webhook cần xác nhận</param>
        /// <returns>Kết quả xác nhận</returns>
        /// <response code="200">Xác nhận webhook URL thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPost("confirm-webhook")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ConfirmWebhook([FromBody] string webhookUrl)
        {
            try
            {
                _logger.LogInformation("Confirming webhook URL: {WebhookUrl}", webhookUrl);

                var result = await _payosService.ConfirmWebhookAsync(webhookUrl);
                return StatusCode((int )result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming webhook URL");
                return StatusCode(500, new { success = false, message = "Lỗi khi xác nhận webhook URL" });
            }
        }
    }
}
