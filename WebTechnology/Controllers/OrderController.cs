using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Lấy thông tin đơn hàng theo ID
        /// </summary>
        /// <remarks>
        /// API này trả về thông tin chi tiết của một đơn hàng dựa trên ID.
        ///
        /// Quyền truy cập:
        /// - Admin: Có thể xem tất cả đơn hàng
        /// - Khách hàng: Chỉ có thể xem đơn hàng của mình
        ///
        /// Ví dụ: GET /api/Order/123456
        /// </remarks>
        /// <param name="id">ID của đơn hàng cần xem</param>
        /// <returns>Thông tin chi tiết của đơn hàng</returns>
        /// <response code="200">Trả về thông tin đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetOrderByIdAsync(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy tất cả đơn hàng (dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả đơn hàng trong hệ thống, chỉ dành cho admin.
        ///
        /// Lưu ý: API này trả về tất cả đơn hàng không phân trang, nên có thể chậm nếu có nhiều đơn hàng.
        /// Khuyến nghị sử dụng API phân trang (/api/Order/paginated) thay thế.
        ///
        /// Ví dụ: GET /api/Order
        /// </remarks>
        /// <returns>Danh sách tất cả đơn hàng</returns>
        /// <response code="200">Trả về danh sách đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllOrders()
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetAllOrdersAsync(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng có phân trang, lọc và sắp xếp (dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách đơn hàng có phân trang, cho phép lọc và sắp xếp theo nhiều tiêu chí.
        ///
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng đơn hàng mỗi trang (tối đa 50)
        /// - CustomerId: Lọc theo ID khách hàng
        /// - StatusId: Lọc theo trạng thái đơn hàng
        /// - SearchTerm: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// - StartDate: Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// - EndDate: Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// - SortBy: Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        ///
        /// Ví dụ: GET /api/Order/paginated?PageNumber=1&amp;PageSize=10&amp;StatusId=PENDING
        /// </remarks>
        /// <param name="request">Thông tin yêu cầu phân trang, lọc và sắp xếp</param>
        /// <returns>Danh sách đơn hàng có phân trang</returns>
        /// <response code="200">Trả về danh sách đơn hàng có phân trang</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("paginated")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetPaginatedOrders([FromQuery] OrderQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetPaginatedOrdersAsync(request, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của khách hàng hiện tại
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách tất cả đơn hàng của khách hàng hiện tại (dựa vào token).
        ///
        /// Lưu ý: API này trả về tất cả đơn hàng không phân trang, nên có thể chậm nếu có nhiều đơn hàng.
        /// Khuyến nghị sử dụng API phân trang (/api/Order/history hoặc /api/Order/user/status/{statusId}) thay thế.
        ///
        /// Ví dụ: GET /api/Order/user
        /// </remarks>
        /// <returns>Danh sách đơn hàng của khách hàng hiện tại</returns>
        /// <response code="200">Trả về danh sách đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("user")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetOrdersByUserIdAsync(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng tạo đơn hàng mới.
        ///
        /// Cấu trúc dữ liệu gửi lên:
        /// - ShippingAddress: Địa chỉ giao hàng (bắt buộc)
        /// - ShippingFee: Phí vận chuyển
        /// - ShippingCode: Mã vận đơn
        /// - PaymentMethod: Phương thức thanh toán (bắt buộc)
        /// - Notes: Ghi chú đơn hàng
        /// - StatusId: Trạng thái đơn hàng (mặc định là PENDING)
        /// - VoucherCodes: Danh sách mã giảm giá
        /// - OrderDetails: Danh sách sản phẩm trong đơn hàng (bắt buộc)
        ///   + ProductId: ID sản phẩm (bắt buộc)
        ///   + Quantity: Số lượng (bắt buộc)
        ///
        /// Lưu ý:
        /// - Hệ thống sẽ tự động kiểm tra tồn kho và tính giá sản phẩm
        /// - Nếu sử dụng voucher, hệ thống sẽ tự động áp dụng và tính giảm giá
        ///
        /// Ví dụ:
        /// ```json
        /// {
        ///   "shippingAddress": "123 Đường ABC, Quận 1, TP.HCM",
        ///   "shippingFee": 30000,
        ///   "paymentMethod": "CASH",
        ///   "notes": "Giao hàng giờ hành chính",
        ///   "voucherCodes": ["SUMMER2023"],
        ///   "orderDetails": [
        ///     {
        ///       "productId": "123456",
        ///       "quantity": 2
        ///     }
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <param name="orderRequest">Thông tin đơn hàng cần tạo</param>
        /// <returns>Thông tin đơn hàng đã tạo</returns>
        /// <response code="200">Đơn hàng được tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO orderRequest)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.CreateOrderAsync(orderRequest, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật thông tin đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép cập nhật thông tin đơn hàng hiện có.
        ///
        /// Quyền truy cập:
        /// - Admin: Có thể cập nhật tất cả đơn hàng
        /// - Khách hàng: Chỉ có thể cập nhật đơn hàng của mình và chỉ khi đơn hàng chưa được xác nhận
        ///
        /// Cấu trúc dữ liệu gửi lên giống với tạo đơn hàng mới.
        ///
        /// Lưu ý:
        /// - Nếu thay đổi số lượng sản phẩm, hệ thống sẽ tự động cập nhật tồn kho
        /// - Không thể cập nhật đơn hàng đã hoàn thành hoặc đã hủy
        ///
        /// Ví dụ: PUT /api/Order/123456
        /// </remarks>
        /// <param name="id">ID của đơn hàng cần cập nhật</param>
        /// <param name="orderRequest">Thông tin đơn hàng cập nhật</param>
        /// <returns>Thông tin đơn hàng đã cập nhật</returns>
        /// <response code="200">Đơn hàng được cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderRequestDTO orderRequest)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.UpdateOrderAsync(id, orderRequest, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép xóa đơn hàng khỏi hệ thống.
        ///
        /// Quyền truy cập:
        /// - Admin: Có thể xóa tất cả đơn hàng
        /// - Khách hàng: Chỉ có thể xóa đơn hàng của mình và chỉ khi đơn hàng chưa được xác nhận
        ///
        /// Lưu ý:
        /// - Khi xóa đơn hàng, hệ thống sẽ tự động hoàn lại số lượng tồn kho
        /// - Thay vì xóa đơn hàng, khuyến nghị sử dụng API hủy đơn hàng (/api/Order/{orderId}/cancel)
        ///
        /// Ví dụ: DELETE /api/Order/123456
        /// </remarks>
        /// <param name="id">ID của đơn hàng cần xóa</param>
        /// <returns>Kết quả xóa đơn hàng</returns>
        /// <response code="200">Đơn hàng được xóa thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.DeleteOrderAsync(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng (dành cho admin và staff)
        /// </summary>
        /// <remarks>
        /// API này cho phép admin và staff cập nhật trạng thái của đơn hàng theo quy trình chuẩn.
        ///
        /// Giá trị statusId:
        /// - PENDING: Chờ xác nhận
        /// - CONFIRMED: Đã xác nhận
        /// - SHIPPING: Đang giao hàng
        /// - COMPLETED: Đã hoàn thành
        /// - CANCELLED: Đã hủy
        ///
        /// Quy trình cập nhật trạng thái:
        /// - PENDING -> CONFIRMED hoặc CANCELLED
        /// - CONFIRMED -> PROCESSING hoặc CANCELLED
        /// - PROCESSING -> SHIPPING
        /// - SHIPPING -> COMPLETED
        /// - Không thể cập nhật đơn hàng đã ở trạng thái COMPLETED hoặc CANCELLED
        ///
        /// Lưu ý:
        /// - Khi cập nhật trạng thái thành COMPLETED, hệ thống sẽ tự động cập nhật số lượng đã bán (SoldQuantity) của sản phẩm
        /// - Khi cập nhật trạng thái thành CANCELLED, hệ thống sẽ tự động hoàn lại số lượng tồn kho
        ///
        /// Ví dụ: PUT /api/Order/123456/status/CONFIRMED
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần cập nhật trạng thái</param>
        /// <param name="statusId">ID trạng thái mới</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        /// <response code="200">Trạng thái đơn hàng được cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc không tuân theo quy trình cập nhật</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin hoặc staff</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{orderId}/status/{statusId}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string statusId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.UpdateOrderStatusAsync(orderId, statusId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng bất kỳ (chỉ dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này cho phép admin cập nhật trạng thái của đơn hàng sang bất kỳ trạng thái nào, bỏ qua các quy tắc kiểm tra thông thường.
        ///
        /// Giá trị statusId:
        /// - PENDING: Chờ xác nhận
        /// - CONFIRMED: Đã xác nhận
        /// - PROCESSING: Đang xử lý
        /// - SHIPPING: Đang giao hàng
        /// - COMPLETED: Đã hoàn thành
        /// - CANCELLED: Đã hủy
        ///
        /// Đặc điểm:
        /// - Không kiểm tra quy trình cập nhật trạng thái
        /// - Có thể cập nhật đơn hàng đã ở trạng thái COMPLETED hoặc CANCELLED
        /// - Chỉ admin mới có quyền sử dụng API này
        ///
        /// Lưu ý:
        /// - Khi cập nhật trạng thái thành COMPLETED, hệ thống vẫn tự động cập nhật số lượng đã bán (SoldQuantity) của sản phẩm
        /// - Khi cập nhật trạng thái thành CANCELLED, hệ thống vẫn tự động hoàn lại số lượng tồn kho
        /// - Hệ thống sẽ lưu lịch sử cập nhật trạng thái
        ///
        /// Ví dụ: PUT /api/Order/123456/admin-status/CONFIRMED
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần cập nhật trạng thái</param>
        /// <param name="statusId">ID trạng thái mới</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        /// <response code="200">Trạng thái đơn hàng được cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{orderId}/admin-status/{statusId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AdminUpdateOrderStatus(string orderId, string statusId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.AdminUpdateOrderStatusAsync(orderId, statusId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Tính tổng tiền đơn hàng (dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này cho phép admin tính tổng tiền của một đơn hàng.
        ///
        /// Tổng tiền bao gồm:
        /// - Tổng giá trị sản phẩm
        /// - Trừ giảm giá (nếu có)
        /// - Cộng phí vận chuyển (nếu có)
        ///
        /// Ví dụ: GET /api/Order/123456/total
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần tính tổng tiền</param>
        /// <returns>Tổng tiền đơn hàng</returns>
        /// <response code="200">Trả về tổng tiền đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{orderId}/total")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CalculateOrderTotal(string orderId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.CalculateOrderTotalAsync(orderId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Hủy đơn hàng (dành cho khách hàng)
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng hủy đơn hàng của mình.
        ///
        /// Lưu ý:
        /// - Chỉ có thể hủy đơn hàng ở trạng thái PENDING hoặc CONFIRMED
        /// - Không thể hủy đơn hàng đã hoàn thành (COMPLETED) hoặc đang giao hàng (SHIPPING)
        /// - Khi hủy đơn hàng, hệ thống sẽ tự động hoàn lại số lượng tồn kho
        /// - Trạng thái đơn hàng sẽ được cập nhật thành CANCELLED
        ///
        /// Ví dụ: POST /api/Order/123456/cancel
        /// </remarks>
        /// <param name="orderId">ID của đơn hàng cần hủy</param>
        /// <returns>Kết quả hủy đơn hàng</returns>
        /// <response code="200">Đơn hàng được hủy thành công</response>
        /// <response code="400">Không thể hủy đơn hàng (đã hoàn thành hoặc đang giao)</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải đơn hàng của khách hàng hiện tại</response>
        /// <response code="404">Không tìm thấy đơn hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("{orderId}/cancel")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.CancelOrderAndRestoreStockAsync(orderId, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy lịch sử đơn hàng thành công của khách hàng hiện tại
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các đơn hàng thành công (IsSuccess = true) của khách hàng hiện tại, có phân trang.
        ///
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng đơn hàng mỗi trang (tối đa 50)
        /// - SearchTerm: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// - StartDate: Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// - EndDate: Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// - SortBy: Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        ///
        /// Ví dụ: GET /api/Order/history?PageNumber=1&amp;PageSize=10&amp;StartDate=2023-01-01&amp;EndDate=2023-12-31
        /// </remarks>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Danh sách đơn hàng thành công có phân trang</returns>
        /// <response code="200">Trả về danh sách đơn hàng thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("history")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetCustomerOrderHistory([FromQuery] OrderHistoryQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _orderService.GetCustomerOrderHistoryAsync(request, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy lịch sử đơn hàng của một khách hàng cụ thể (dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này cho phép admin xem lịch sử đơn hàng của một khách hàng cụ thể, có phân trang.
        ///
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng đơn hàng mỗi trang (tối đa 50)
        /// - OnlySuccessful: Chỉ lấy đơn hàng thành công (true/false)
        /// - StatusId: Lọc theo trạng thái đơn hàng
        /// - SearchTerm: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// - StartDate: Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// - EndDate: Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// - SortBy: Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        ///
        /// Ví dụ: GET /api/Order/customer/123456/history?PageNumber=1&amp;PageSize=10&amp;OnlySuccessful=true
        /// </remarks>
        /// <param name="customerId">ID của khách hàng cần xem lịch sử</param>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Danh sách đơn hàng của khách hàng có phân trang</returns>
        /// <response code="200">Trả về danh sách đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("customer/{customerId}/history")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetCustomerOrderHistoryByAdmin(string customerId, [FromQuery] OrderHistoryQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();

            // Gán customerId từ đường dẫn vào request
            request.CustomerId = customerId;

            var response = await _orderService.GetCustomerOrderHistoryByAdminAsync(request, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo trạng thái có phân trang (dành cho admin)
        /// </summary>
        /// <remarks>
        /// API này cho phép admin xem danh sách đơn hàng theo trạng thái cụ thể, có phân trang.
        ///
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng đơn hàng mỗi trang (tối đa 50)
        /// - CustomerId: Lọc theo ID khách hàng
        /// - SearchTerm: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// - StartDate: Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// - EndDate: Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// - SortBy: Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        ///
        /// Giá trị statusId:
        /// - PENDING: Chờ xác nhận
        /// - CONFIRMED: Đã xác nhận
        /// - SHIPPING: Đang giao hàng
        /// - COMPLETED: Đã hoàn thành
        /// - CANCELLED: Đã hủy
        ///
        /// Ví dụ: GET /api/Order/status/PENDING?PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="statusId">ID trạng thái đơn hàng</param>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Danh sách đơn hàng theo trạng thái có phân trang</returns>
        /// <response code="200">Trả về danh sách đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không phải admin</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("status/{statusId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrdersByStatus(string statusId, [FromQuery] OrderQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();

            // Gán statusId từ đường dẫn vào request
            request.StatusId = statusId;

            var response = await _orderService.GetPaginatedOrdersAsync(request, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của khách hàng hiện tại theo trạng thái
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng xem danh sách đơn hàng của mình theo trạng thái cụ thể, có phân trang.
        ///
        /// Các tham số:
        /// - PageNumber: Số trang (bắt đầu từ 1)
        /// - PageSize: Số lượng đơn hàng mỗi trang (tối đa 50)
        /// - SearchTerm: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// - StartDate: Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// - EndDate: Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// - SortBy: Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        ///
        /// Giá trị statusId:
        /// - PENDING: Chờ xác nhận
        /// - CONFIRMED: Đã xác nhận
        /// - SHIPPING: Đang giao hàng
        /// - COMPLETED: Đã hoàn thành
        /// - CANCELLED: Đã hủy
        ///
        /// Ví dụ: GET /api/Order/user/status/PENDING?PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="statusId">ID trạng thái đơn hàng</param>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Danh sách đơn hàng của khách hàng theo trạng thái có phân trang</returns>
        /// <response code="200">Trả về danh sách đơn hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("user/status/{statusId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetCustomerOrdersByStatus(string statusId, [FromQuery] OrderQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();

            // Gán statusId từ đường dẫn vào request
            request.StatusId = statusId;

            var response = await _orderService.GetCustomerPaginatedOrdersAsync(request, token);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
