using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;
using Net.payOS;
using Net.payOS.Types;
using Net.payOS.Errors;

namespace WebTechnology.Service.Services.Implementations
{
    /// <summary>
    /// Dịch vụ thanh toán Payos
    /// </summary>
    public class PayosService : IPayosService
    {
        private readonly PayosSettings _payosSettings;
        private readonly ILogger<PayosService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payOS; // Đối tượng PayOS từ thư viện

        public PayosService(
            IOptions<PayosSettings> payosSettings,
            ILogger<PayosService> logger,
            HttpClient httpClient,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _payosSettings = payosSettings.Value;
            _logger = logger;
            _httpClient = httpClient;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;

            // Khởi tạo đối tượng PayOS từ thư viện
            _payOS = new PayOS(
                _payosSettings.ClientId,
                _payosSettings.ApiKey,
                _payosSettings.ChecksumKey
            );

            // Cấu hình HttpClient cho các trường hợp cần sử dụng trực tiếp
            _httpClient.BaseAddress = new Uri(_payosSettings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _payosSettings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _payosSettings.ApiKey);
        }

        /// <summary>
        /// Tạo link thanh toán Payos
        /// </summary>
        /// <param name="request">Thông tin thanh toán</param>
        /// <returns>Thông tin link thanh toán</returns>
        public async Task<ServiceResponse<PayosPaymentData>> CreatePaymentLinkAsync(PayosCreatePaymentLinkRequest request)
        {
            try
            {
                _logger.LogInformation("Creating Payos payment link for order {OrderId}", request.OrderId);

                // Lấy thông tin đơn hàng từ OrderId
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return ServiceResponse<PayosPaymentData>.NotFoundResponse("Không tìm thấy đơn hàng");
                }

                // Lấy số tiền từ đơn hàng
                int amount = 0;
                if (order.TotalPrice.HasValue)
                {
                    amount = (int)order.TotalPrice.Value;
                }
                else
                {
                    return ServiceResponse<PayosPaymentData>.ErrorResponse("Đơn hàng không có thông tin giá");
                }

                // Lấy mã đơn hàng từ OrderNumber, bỏ tiền tố "ORD-"
                string orderCode = order.OrderNumber?.Replace("ORD-", "") ?? DateTime.Now.Ticks.ToString();

                // Chuyển đổi orderCode từ chuỗi thành số
                int numericOrderCode;
                if (!int.TryParse(orderCode, out numericOrderCode))
                {
                    // Nếu không thể chuyển đổi, sử dụng một số ngẫu nhiên
                    Random random = new Random();
                    numericOrderCode = random.Next(10000000, 99999999);
                    _logger.LogWarning("Could not parse orderCode {OrderCode} to number, using random number {RandomNumber}",
                        orderCode, numericOrderCode);
                }

                // Giới hạn mô tả tối đa 25 ký tự
                string description = "Thanh toán đơn hàng";

                // Tạo item data
                var itemName = $"Đơn hàng #{order.OrderNumber}";
                var itemQuantity = 1;
                var itemPrice = amount;

                var item = new ItemData(itemName, itemQuantity, itemPrice);
                var items = new List<ItemData>();
                items.Add(item);

                // Tạo payment data theo cách thư viện payOS yêu cầu
                var paymentData = new PaymentData(
                    (int)numericOrderCode,  // Chuyển đổi sang int theo yêu cầu của thư viện
                    amount,
                    description,
                    items,
                    request.CancelUrl,
                    request.ReturnUrl
                );

                try
                {
                    // Gọi API tạo payment link với paymentData
                    var paymentLinkResponse = await _payOS.createPaymentLink(paymentData);
                    _logger.LogInformation("Payos response: {Response}", JsonConvert.SerializeObject(paymentLinkResponse));

                    if (paymentLinkResponse == null)
                    {
                        _logger.LogError("Payos error: Null response");
                        return ServiceResponse<PayosPaymentData>.ErrorResponse("Không nhận được phản hồi từ Payos");
                    }

                    // Lưu thông tin paymentLinkId vào đơn hàng để dễ dàng tra cứu sau này
                    order.PaymentLinkId = paymentLinkResponse.paymentLinkId?.ToString() ?? "";
                    // Cập nhật đơn hàng
                    await _orderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();

                    // Log chi tiết response để debug
                    _logger.LogInformation("PayOS response details: paymentLinkId={PaymentLinkId}, checkoutUrl={CheckoutUrl}, expiredAt={ExpiredAt}",
                        paymentLinkResponse.paymentLinkId,
                        paymentLinkResponse.checkoutUrl,
                        paymentLinkResponse.expiredAt);

                    // Chuyển đổi data thành PayosPaymentData, xử lý các trường có thể null
                    var paymentResult = new PayosPaymentData
                    {
                        PaymentLinkId = paymentLinkResponse.paymentLinkId?.ToString() ?? "",
                        CheckoutUrl = paymentLinkResponse.checkoutUrl ?? "",
                        QrCode = paymentLinkResponse.qrCode ?? "",
                        ExpiredAt = paymentLinkResponse.expiredAt.HasValue ? paymentLinkResponse.expiredAt.Value : 0,
                        OrderCode = paymentLinkResponse.orderCode.ToString() ?? "",
                        Amount = paymentLinkResponse.amount,
                        Description = paymentLinkResponse.description ?? "",
                        Status = paymentLinkResponse.status ?? "PENDING"
                    };

                    return ServiceResponse<PayosPaymentData>.SuccessResponse(
                        paymentResult,
                        "Tạo link thanh toán thành công");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Payos response: {Message}", ex.Message);
                    return ServiceResponse<PayosPaymentData>.ErrorResponse($"Lỗi khi xử lý phản hồi từ Payos: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Payos payment link");
                return ServiceResponse<PayosPaymentData>.ErrorResponse($"Lỗi khi tạo link thanh toán: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý webhook từ Payos
        /// </summary>
        /// <param name="webhookRequest">Dữ liệu webhook</param>
        /// <returns>Kết quả xử lý</returns>
        public async Task<ServiceResponse<bool>> ProcessWebhookAsync(PayosWebhookRequest webhookRequest)
        {
            try
            {
                // Log toàn bộ dữ liệu webhook để debug
                var webhookJson = JsonConvert.SerializeObject(webhookRequest);
                _logger.LogInformation("Received Payos webhook: {WebhookData}", webhookJson);

                // Kiểm tra xem webhookRequest và Data có null không
                if (webhookRequest == null)
                {
                    _logger.LogWarning("Webhook request is null");
                    return ServiceResponse<bool>.ErrorResponse("Dữ liệu webhook trống");
                }

                if (webhookRequest.Data == null)
                {
                    _logger.LogWarning("Webhook data is null");
                    return ServiceResponse<bool>.ErrorResponse("Dữ liệu webhook không hợp lệ");
                }

                _logger.LogInformation("Processing Payos webhook for order {OrderId}", webhookRequest.Data.OrderCode);

                // Xác thực chữ ký sử dụng thư viện PayOS
                bool isValidSignature = false;
                try {
                    // Chuyển đổi từ PayosWebhookRequest sang WebhookType của thư viện PayOS
                    var webhookType = PayosWebhookType.FromPayosWebhookRequest(webhookRequest);

                    // Sử dụng phương thức verifyPaymentWebhookData từ thư viện PayOS
                    var webhookData = _payOS.verifyPaymentWebhookData(webhookType);
                    isValidSignature = true;
                    _logger.LogInformation("Webhook signature validated successfully using PayOS library");
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Error validating webhook signature using PayOS library, falling back to custom implementation: {Message}", ex.Message);

                    // Fallback: Sử dụng phương thức tự triển khai để xác thực chữ ký
                    var dataJson = JsonConvert.SerializeObject(webhookRequest.Data);
                    var signature = webhookRequest.Signature;
                    var expectedSignature = GenerateHmacSha256(dataJson, _payosSettings.ChecksumKey);
                    isValidSignature = (expectedSignature == signature);
                }

                _logger.LogInformation("Webhook signature validation: {IsValid}", isValidSignature);

                // Kiểm tra chữ ký - bắt buộc trong môi trường sản xuất
                if (!isValidSignature)
                {
                    _logger.LogWarning("Invalid Payos webhook signature");
                    return ServiceResponse<bool>.ErrorResponse("Chữ ký không hợp lệ");
                }

                // Kiểm tra trạng thái thanh toán
                if (webhookRequest.Data.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase))
                {
                    // Cập nhật trạng thái đơn hàng
                    await _unitOfWork.BeginTransactionAsync();

                    // Lấy OrderCode từ webhook
                    string orderCode = webhookRequest.Data.OrderCode;
                    _logger.LogInformation("Received webhook for OrderCode: {OrderCode}", orderCode);

                    // Tìm đơn hàng theo OrderNumber (thêm tiền tố "ORD-")
                    string exactOrderNumber = $"ORD-{orderCode}";
                    var orders = await _orderRepository.FindAsync(o => o.OrderNumber == exactOrderNumber);
                    var order = orders.FirstOrDefault();

                    if (order == null)
                    {
                        return ServiceResponse<bool>.NotFoundResponse($"Không tìm thấy đơn hàng với mã {exactOrderNumber}");
                    }

                    _logger.LogInformation("Found order: ID={OrderId}, Number={OrderNumber}, Status={Status}",
                        order.Orderid, order.OrderNumber, order.StatusId);

                    // Cập nhật trạng thái đơn hàng thành đã thanh toán
                    order.IsSuccess = true;

                    await _orderRepository.UpdateAsync(order);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Order {OrderId} with code {OrderCode} payment status updated to PAID", order.Orderid, orderCode);
                    return ServiceResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái thanh toán thành công");
                }

                return ServiceResponse<bool>.SuccessResponse(false, "Trạng thái thanh toán chưa hoàn thành");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error processing Payos webhook");
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi xử lý webhook: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán
        /// </summary>
        /// <param name="paymentLinkId">ID giao dịch trong hệ thống Payos</param>
        /// <returns>Thông tin trạng thái thanh toán</returns>
        public async Task<ServiceResponse<string>> CheckPaymentStatusAsync(string paymentLinkId)
        {
            try
            {
                _logger.LogInformation("Checking payment status for Payos payment {PaymentLinkId}", paymentLinkId);

                // Sử dụng thư viện payOS để kiểm tra trạng thái thanh toán
                try
                {
                    // Gọi API để kiểm tra trạng thái thanh toán
                    _logger.LogInformation("Checking payment status using payOS library for paymentLinkId: {PaymentLinkId}", paymentLinkId);

                    // Chuyển đổi paymentLinkId từ string sang long
                    if (!long.TryParse(paymentLinkId, out long orderId))
                    {
                        return ServiceResponse<string>.ErrorResponse("Mã giao dịch không hợp lệ, không thể chuyển đổi sang số");
                    }

                    var paymentResponse = await _payOS.getPaymentLinkInformation(orderId);

                    // Log response để debug
                    _logger.LogInformation("Payos check status response: {Response}", JsonConvert.SerializeObject(paymentResponse));

                    if (paymentResponse == null)
                    {
                        return ServiceResponse<string>.ErrorResponse("Không nhận được phản hồi từ Payos");
                    }

                    // PaymentLinkInformation không có thuộc tính code/message
                    // Nếu đã nhận được response thì coi như thành công
                    string status = paymentResponse.status ?? "UNKNOWN";
                    return ServiceResponse<string>.SuccessResponse(status, "Kiểm tra trạng thái thanh toán thành công");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking payment status using payOS library: {Message}", ex.Message);
                    return ServiceResponse<string>.ErrorResponse($"Lỗi khi xử lý phản hồi từ Payos: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Payos payment status");
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi kiểm tra trạng thái thanh toán: {ex.Message}");
            }
        }

        /// <summary>
        /// Xác nhận webhook URL với Payos
        /// </summary>
        /// <param name="webhookUrl">URL webhook cần xác nhận</param>
        /// <returns>Kết quả xác nhận</returns>
        public async Task<ServiceResponse<bool>> ConfirmWebhookAsync(string webhookUrl)
        {
            try
            {
                _logger.LogInformation("Confirming webhook URL with Payos: {WebhookUrl}", webhookUrl);

                // Sử dụng thư viện payOS để xác nhận webhook URL
                await _payOS.confirmWebhook(webhookUrl);

                return ServiceResponse<bool>.SuccessResponse(true, "Xác nhận webhook URL thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming webhook URL: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi xác nhận webhook URL: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo HMAC SHA256
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <param name="key">Khóa</param>
        /// <returns>Chữ ký</returns>
        private string GenerateHmacSha256(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
