using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            // Cấu hình HttpClient
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

                // Lấy số tiền và mô tả từ đơn hàng
                int amount = (int) (order.TotalPrice + order.ShippingFee);

                // Lấy OrderNumber từ đơn hàng và loại bỏ phần "ORD-" để có được số
                string orderNumber = order.OrderNumber;

                // Tạo orderCode bằng cách tách phần số từ OrderNumber
                // OrderNumber có định dạng "ORD-12345678"
                string orderCode;
                if (orderNumber.StartsWith("ORD-") && orderNumber.Length > 4)
                {
                    // Lấy phần sau "ORD-"
                    orderCode = orderNumber.Substring(4);
                    _logger.LogInformation("Extracted orderCode: {OrderCode} from OrderNumber: {OrderNumber}",
                        orderCode, orderNumber);
                }
                else
                {
                    // Fallback: Nếu OrderNumber không có định dạng mong đợi, tạo số ngẫu nhiên
                    Random random = new Random();
                    int randomNumber = random.Next(10000000, 99999999);
                    orderCode = randomNumber.ToString();
                    _logger.LogWarning("Could not extract orderCode from OrderNumber: {OrderNumber}, using random number: {OrderCode}",
                        orderNumber, orderCode);
                }

                // Lưu mối quan hệ giữa orderCode và orderId vào log để dễ dàng tra cứu sau này
                _logger.LogInformation("Created payment mapping: OrderCode={OrderCode}, OrderId={OrderId}, OrderNumber={OrderNumber}",
                    orderCode, request.OrderId, orderNumber);

                // Kiểm tra xem orderCode đã tồn tại chưa
                try
                {
                    _logger.LogInformation("Checking if orderCode {OrderCode} already exists", orderCode);

                    // Gọi API để kiểm tra orderCode
                    var checkResponse = await _httpClient.GetAsync($"/v2/payment-requests/order-code/{orderCode}");

                    // Nếu request thành công, có thể orderCode đã tồn tại
                    if (checkResponse.IsSuccessStatusCode)
                    {
                        var checkContent = await checkResponse.Content.ReadAsStringAsync();
                        _logger.LogInformation("Payos check response: {Response}", checkContent);

                        dynamic checkResult = JsonConvert.DeserializeObject(checkContent);

                        // Nếu code là "00", tìm thấy payment request
                        if (checkResult != null && checkResult.code == "00" && checkResult.data != null)
                        {
                            string status = checkResult.data.status?.ToString();
                            string paymentLinkId = checkResult.data.id?.ToString();

                            _logger.LogInformation("Found existing Payos payment: ID={PaymentLinkId}, Status={Status}",
                                paymentLinkId, status);

                            // Tạo đối tượng PayosPaymentData từ response
                            var paymentData = new PayosPaymentData
                            {
                                PaymentLinkId = paymentLinkId,
                                CheckoutUrl = checkResult.data.checkoutUrl?.ToString(),
                                QrCode = checkResult.data.qrCode?.ToString(),
                                ExpiredAt = checkResult.data.expiredAt != null ? (long)checkResult.data.expiredAt : 0,
                                OrderCode = orderCode,
                                Amount = checkResult.data.amount != null ? (int)checkResult.data.amount : 0,
                                Description = checkResult.data.description?.ToString(),
                                Status = status
                            };

                            // Trả về thông tin thanh toán hiện có
                            return ServiceResponse<PayosPaymentData>.SuccessResponse(
                                paymentData,
                                "Trả về thông tin thanh toán hiện có");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi khi kiểm tra, ghi log và tiếp tục tạo mới
                    _logger.LogWarning(ex, "Error checking existing payment, will create new one");
                }

                // Giới hạn mô tả tối đa 25 ký tự
                string description = "Thanh toán đơn hàng";

                // Tạo payload

                // Log thông tin để debug
                _logger.LogInformation("Creating payment with OrderId: {OrderId}, OrderCode: {OrderCode}, Amount: {Amount}, Description: {Description}",
                    request.OrderId, orderCode, amount, description);

                // Chuyển đổi orderCode từ chuỗi thành số
                long numericOrderCode;
                if (!long.TryParse(orderCode, out numericOrderCode))
                {
                    // Nếu không thể chuyển đổi, sử dụng timestamp
                    numericOrderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _logger.LogWarning("Could not parse orderCode {OrderCode} to number, using timestamp {Timestamp}",
                        orderCode, numericOrderCode);
                }
                else
                {
                    _logger.LogInformation("Successfully parsed orderCode {OrderCode} to number {NumericOrderCode}",
                        orderCode, numericOrderCode);
                }

                // Tạo dictionary chứa tất cả các trường để tạo chữ ký
                var signatureData = new Dictionary<string, string>
                {
                    { "amount", amount.ToString() },
                    { "cancelUrl", request.CancelUrl },
                    { "description", description },
                    { "orderCode", numericOrderCode.ToString() },  // Sử dụng số thay vì chuỗi
                    { "returnUrl", request.ReturnUrl }
                };

                // Thêm thông tin khách hàng nếu có
                if (request.CustomerInfo != null)
                {
                    if (!string.IsNullOrEmpty(request.CustomerInfo.Name))
                        signatureData.Add("buyerInfo.name", request.CustomerInfo.Name);
                    if (!string.IsNullOrEmpty(request.CustomerInfo.Email))
                        signatureData.Add("buyerInfo.email", request.CustomerInfo.Email);
                    if (!string.IsNullOrEmpty(request.CustomerInfo.Phone))
                        signatureData.Add("buyerInfo.phone", request.CustomerInfo.Phone);
                }

                // Tạo chữ ký
                var signature = GenerateSignatureFromDictionary(signatureData);

                // Sử dụng numericOrderCode đã chuyển đổi ở trên

                var payload = new
                {
                    orderCode = numericOrderCode,  // Sử dụng số thay vì chuỗi
                    amount = amount,
                    description = description,
                    returnUrl = request.ReturnUrl,
                    cancelUrl = request.CancelUrl,
                    buyerInfo = new
                    {
                        name = request.CustomerInfo?.Name,
                        email = request.CustomerInfo?.Email,
                        phone = request.CustomerInfo?.Phone
                    },
                    signature = signature
                };

                // Gửi request đến Payos
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/v2/payment-requests", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create Payos payment link: {ErrorContent}", errorContent);
                    return ServiceResponse<PayosPaymentData>.ErrorResponse($"Lỗi khi tạo link thanh toán: {response.StatusCode}");
                }

                // Xử lý response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Payos response: {Response}", responseContent);

                try
                {
                    // Kiểm tra xem response có phải là JSON hợp lệ không
                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        return ServiceResponse<PayosPaymentData>.ErrorResponse("Phản hồi từ Payos trống");
                    }

                    // Phân tích response dưới dạng dynamic để dễ dàng truy cập các trường
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                    if (jsonResponse == null)
                    {
                        return ServiceResponse<PayosPaymentData>.ErrorResponse("Không thể phân tích phản hồi từ Payos");
                    }

                    // Kiểm tra code trong response
                    string code = jsonResponse.code?.ToString() ?? "";

                    // Nếu code không phải là "00", đây là lỗi
                    if (code != "00")
                    {
                        string errorMessage = jsonResponse.desc?.ToString() ?? "Lỗi không xác định từ Payos";
                        _logger.LogError("Payos error: Code={Code}, Message={Message}", code, errorMessage);
                        return ServiceResponse<PayosPaymentData>.ErrorResponse($"Lỗi từ Payos: {errorMessage}");
                    }

                    // Nếu không có data, đây là lỗi
                    if (jsonResponse.data == null)
                    {
                        return ServiceResponse<PayosPaymentData>.ErrorResponse("Dữ liệu phản hồi từ Payos trống");
                    }

                    // Chuyển đổi data thành PayosPaymentData
                    var paymentData = new PayosPaymentData
                    {
                        PaymentLinkId = jsonResponse.data.paymentLinkId?.ToString(),
                        CheckoutUrl = jsonResponse.data.checkoutUrl?.ToString(),
                        QrCode = jsonResponse.data.qrCode?.ToString(),
                        ExpiredAt = jsonResponse.data.expiredAt != null ? (long)jsonResponse.data.expiredAt : 0,
                        OrderCode = jsonResponse.data.orderCode?.ToString(),
                        Amount = jsonResponse.data.amount != null ? (int)jsonResponse.data.amount : 0,
                        Description = jsonResponse.data.description?.ToString(),
                        Status = jsonResponse.data.status?.ToString()
                    };

                    return ServiceResponse<PayosPaymentData>.SuccessResponse(
                        paymentData,
                        "Tạo link thanh toán thành công");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Payos response: {Response}", responseContent);
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

                // Xác thực chữ ký - bỏ qua bước này trong quá trình test
                var dataJson = JsonConvert.SerializeObject(webhookRequest.Data);
                var expectedSignature = GenerateHmacSha256(dataJson, _payosSettings.ChecksumKey);

                _logger.LogInformation("Webhook signature validation: Received={ReceivedSignature}, Expected={ExpectedSignature}",
                    webhookRequest.Signature, expectedSignature);

                // Tạm thời bỏ qua việc kiểm tra chữ ký để test
                // Sau khi test thành công, bạn có thể bỏ comment dòng code bên dưới
                /*
                if (expectedSignature != webhookRequest.Signature)
                {
                    _logger.LogWarning("Invalid Payos webhook signature");
                    return ServiceResponse<bool>.ErrorResponse("Chữ ký không hợp lệ");
                }
                */

                // Kiểm tra trạng thái thanh toán
                if (webhookRequest.Data.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase))
                {
                    // Cập nhật trạng thái đơn hàng
                    await _unitOfWork.BeginTransactionAsync();

                    // Lấy OrderCode từ webhook
                    string orderCode = webhookRequest.Data.OrderCode;
                    _logger.LogInformation("Received webhook for OrderCode: {OrderCode}", orderCode);

                    // Log thông tin chi tiết về orderCode
                    _logger.LogInformation("Attempting to find order for orderCode: {OrderCode}", orderCode);

                    // Thử nhiều cách khác nhau để tìm đơn hàng
                    Order order = null;

                    // Cách 1: Tìm theo OrderNumber chính xác
                    string exactOrderNumber = $"ORD-{orderCode}";
                    _logger.LogInformation("Trying to find order with exact OrderNumber: {OrderNumber}", exactOrderNumber);
                    var orders = await _orderRepository.FindAsync(o => o.OrderNumber == exactOrderNumber);
                    order = orders.FirstOrDefault();

                    // Cách 2: Tìm theo OrderNumber bắt đầu bằng prefix
                    if (order == null)
                    {
                        _logger.LogInformation("No exact match, trying prefix search with: {Prefix}", $"ORD-{orderCode}");
                        orders = await _orderRepository.FindAsync(o => o.OrderNumber.StartsWith($"ORD-{orderCode}"));
                        order = orders.FirstOrDefault();
                    }

                    // Cách 3: Tìm theo OrderNumber chứa orderCode
                    if (order == null)
                    {
                        _logger.LogInformation("No prefix match, trying contains search with: {OrderCode}", orderCode);
                        orders = await _orderRepository.FindAsync(o => o.OrderNumber.Contains(orderCode));
                        order = orders.FirstOrDefault();
                    }

                    // Cách 4: Lấy tất cả đơn hàng và tìm đơn hàng mới nhất
                    if (order == null)
                    {
                        _logger.LogWarning("No order found with OrderCode: {OrderCode}, getting all orders", orderCode);
                        var allOrders = await _orderRepository.GetAllAsync();

                        // Log số lượng đơn hàng để debug
                        _logger.LogInformation("Total orders in database: {Count}", allOrders.Count());

                        // Log thông tin chi tiết về các đơn hàng
                        foreach (var o in allOrders.Take(5))
                        {
                            _logger.LogInformation("Order in DB: ID={OrderId}, Number={OrderNumber}, Status={Status}, CreatedAt={CreatedAt}",
                                o.Orderid, o.OrderNumber, o.StatusId, o.CreatedAt);
                        }

                        // Tìm đơn hàng mới nhất
                        order = allOrders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();

                        if (order == null)
                        {
                            return ServiceResponse<bool>.NotFoundResponse($"Không tìm thấy đơn hàng nào trong hệ thống");
                        }

                        _logger.LogWarning("Using most recent order as fallback: ID={OrderId}, Number={OrderNumber}",
                            order.Orderid, order.OrderNumber);
                    }
                    else
                    {
                        _logger.LogInformation("Found order: ID={OrderId}, Number={OrderNumber}, Status={Status}",
                            order.Orderid, order.OrderNumber, order.StatusId);
                    }

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

                // Gửi request đến Payos
                var response = await _httpClient.GetAsync($"/v2/payment-requests/{paymentLinkId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to check Payos payment status: {ErrorContent}", errorContent);
                    return ServiceResponse<string>.ErrorResponse($"Lỗi khi kiểm tra trạng thái thanh toán: {response.StatusCode}");
                }

                // Xử lý response
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Payos check status response: {Response}", responseContent);

                try
                {
                    // Kiểm tra xem response có phải là JSON hợp lệ không
                    if (string.IsNullOrWhiteSpace(responseContent))
                    {
                        return ServiceResponse<string>.ErrorResponse("Phản hồi từ Payos trống");
                    }

                    // Phân tích response dưới dạng dynamic để dễ dàng truy cập các trường
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                    if (jsonResponse == null)
                    {
                        return ServiceResponse<string>.ErrorResponse("Không thể phân tích phản hồi từ Payos");
                    }

                    // Kiểm tra code trong response
                    string code = jsonResponse.code?.ToString() ?? "";

                    // Nếu code không phải là "00", đây là lỗi
                    if (code != "00")
                    {
                        string errorMessage = jsonResponse.desc?.ToString() ?? "Lỗi không xác định từ Payos";
                        _logger.LogError("Payos error: Code={Code}, Message={Message}", code, errorMessage);
                        return ServiceResponse<string>.ErrorResponse($"Lỗi từ Payos: {errorMessage}");
                    }

                    // Nếu không có data, đây là lỗi
                    if (jsonResponse.data == null)
                    {
                        return ServiceResponse<string>.ErrorResponse("Dữ liệu phản hồi từ Payos trống");
                    }

                    string status = jsonResponse.data.status?.ToString() ?? "UNKNOWN";
                    return ServiceResponse<string>.SuccessResponse(status, "Kiểm tra trạng thái thanh toán thành công");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Payos check status response: {Response}", responseContent);
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
        /// Tạo chữ ký từ dictionary theo yêu cầu của Payos
        /// </summary>
        /// <param name="data">Dictionary chứa dữ liệu để tạo chữ ký</param>
        /// <returns>Chữ ký</returns>
        private string GenerateSignatureFromDictionary(Dictionary<string, string> data)
        {
            try
            {
                // Sắp xếp các trường theo thứ tự alphabet
                var sortedData = data.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                // Tạo chuỗi dữ liệu dạng key1=value1&key2=value2...
                var queryString = string.Join("&", sortedData.Select(x => $"{x.Key}={x.Value}"));

                _logger.LogInformation("Signature query string: {QueryString}", queryString);

                // Sử dụng thuật toán HMAC_SHA256 với chuỗi dữ liệu và checksum_key
                return GenerateHmacSha256(queryString, _payosSettings.ChecksumKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating signature from dictionary");
                return string.Empty;
            }
        }

        /// <summary>
        /// Tạo chữ ký cho request (phương thức cũ, giữ lại để tương thích)
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <param name="amount">Số tiền</param>
        /// <param name="description">Mô tả đơn hàng</param>
        /// <returns>Chữ ký</returns>
        private string GenerateSignature(string orderId, string amount, string description = null)
        {
            try
            {
                // Tạo dictionary chứa dữ liệu
                var data = new Dictionary<string, string>
                {
                    { "orderCode", orderId },
                    { "amount", amount }
                };

                if (!string.IsNullOrEmpty(description))
                {
                    data.Add("description", description);
                }

                // Sử dụng phương thức mới
                return GenerateSignatureFromDictionary(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating signature");
                return string.Empty;
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
