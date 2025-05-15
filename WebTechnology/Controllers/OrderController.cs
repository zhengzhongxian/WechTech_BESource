using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    /// <summary>
    /// API quản lý đơn hàng
    /// </summary>
    /// <remarks>
    /// Controller này cung cấp các API để quản lý đơn hàng trong hệ thống, bao gồm:
    /// - Tạo đơn hàng mới
    /// - Xem thông tin đơn hàng
    /// - Cập nhật đơn hàng
    /// - Hủy đơn hàng
    /// - Cập nhật trạng thái đơn hàng
    /// - Xem lịch sử đơn hàng
    ///
    /// Các API được phân quyền theo vai trò:
    /// - Admin: Có thể truy cập tất cả API và thao tác với tất cả đơn hàng
    /// - Staff: Có thể xem và cập nhật trạng thái đơn hàng theo quy trình
    /// - Customer: Chỉ có thể xem và thao tác với đơn hàng của mình
    ///
    /// Tất cả các API đều yêu cầu xác thực bằng token JWT trong header Authorization.
    /// </remarks>
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
        /// **Quyền truy cập:**
        /// - Admin/Staff: Có thể xem tất cả đơn hàng
        /// - Khách hàng: Chỉ có thể xem đơn hàng của mình
        ///
        /// **Cách sử dụng:**
        /// - Gửi request GET đến endpoint với ID đơn hàng
        /// - Thêm token JWT vào header Authorization
        ///
        /// **Kết quả trả về:**
        /// - Thông tin chi tiết đơn hàng bao gồm:
        ///   + Thông tin cơ bản (ID, mã đơn hàng, ngày đặt, địa chỉ giao hàng, v.v.)
        ///   + Danh sách sản phẩm trong đơn hàng (tên, giá, số lượng, hình ảnh)
        ///   + Danh sách voucher đã áp dụng (mã, giá trị giảm giá)
        ///   + Trạng thái đơn hàng
        ///
        /// **Ví dụ request:**
        /// ```
        /// GET /api/Order/123456
        /// Authorization: Bearer {token}
        /// ```
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Lấy thông tin đơn hàng thành công",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "orderId": "123456",
        ///     "orderNumber": "ORD-20230615-001",
        ///     "customerId": "cust123",
        ///     "orderDate": "2023-06-15T10:30:00",
        ///     "shippingAddress": "123 Đường ABC, Quận 1, TP.HCM",
        ///     "shippingFee": 30000,
        ///     "totalPrice": 530000,
        ///     "paymentMethod": "CASH",
        ///     "paymentMethodName": "Tiền mặt",
        ///     "notes": "Giao hàng giờ hành chính",
        ///     "statusId": "CONFIRMED",
        ///     "isSuccess": false,
        ///     "orderDetails": [
        ///       {
        ///         "orderDetailId": "od123",
        ///         "productId": "prod456",
        ///         "productName": "Sữa tươi nguyên kem",
        ///         "productPrice": 250000,
        ///         "quantity": 2,
        ///         "subTotal": 500000,
        ///         "img": "base64-encoded-image-data"
        ///       }
        ///     ],
        ///     "appliedVouchers": [
        ///       {
        ///         "voucherId": "v789",
        ///         "voucherCode": "SUMMER2023",
        ///         "discountValue": 50000,
        ///         "discountType": "0"
        ///       }
        ///     ]
        ///   }
        /// }
        /// ```
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
        /// **Quyền truy cập:**
        /// - Chỉ khách hàng đã đăng nhập mới có thể tạo đơn hàng
        /// - Yêu cầu token JWT trong header Authorization
        ///
        /// **Cấu trúc dữ liệu gửi lên:**
        /// - **shippingAddress**: Địa chỉ giao hàng (bắt buộc)
        /// - **shippingFee**: Phí vận chuyển (có thể để trống, hệ thống sẽ tính toán)
        /// - **shippingCode**: Mã vận đơn (có thể để trống, hệ thống sẽ tự sinh)
        /// - **paymentMethod**: Phương thức thanh toán (bắt buộc, các giá trị: "CASH", "BANKING", "MOMO", "ZALOPAY")
        /// - **notes**: Ghi chú đơn hàng (tùy chọn)
        /// - **voucherCodes**: Danh sách mã giảm giá (tùy chọn, mảng chuỗi)
        /// - **orderDetails**: Danh sách sản phẩm trong đơn hàng (bắt buộc, mảng đối tượng)
        ///   + **productId**: ID sản phẩm (bắt buộc)
        ///   + **quantity**: Số lượng (bắt buộc, phải > 0)
        ///
        /// **Xử lý của hệ thống:**
        /// - Kiểm tra tồn kho: Nếu số lượng yêu cầu vượt quá tồn kho, trả về lỗi 400
        /// - Tính giá sản phẩm: Lấy giá hiện tại của sản phẩm (có tính đến giảm giá nếu có)
        /// - Áp dụng voucher: Kiểm tra tính hợp lệ của voucher và áp dụng giảm giá
        /// - Tính tổng tiền: Tổng giá trị sản phẩm + phí vận chuyển - giảm giá
        /// - Tạo đơn hàng: Lưu thông tin đơn hàng và chi tiết vào database
        /// - Cập nhật tồn kho: Giảm số lượng tồn kho tương ứng
        ///
        /// **Ví dụ request:**
        /// ```
        /// POST /api/Order
        /// Authorization: Bearer {token}
        /// Content-Type: application/json
        ///
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
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Đơn hàng được tạo thành công",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "orderId": "789012",
        ///     "orderNumber": "ORD-20230615-002",
        ///     "customerId": "cust123",
        ///     "orderDate": "2023-06-15T14:45:00",
        ///     "shippingAddress": "123 Đường ABC, Quận 1, TP.HCM",
        ///     "shippingFee": 30000,
        ///     "totalPrice": 530000,
        ///     "paymentMethod": "CASH",
        ///     "paymentMethodName": "Tiền mặt",
        ///     "notes": "Giao hàng giờ hành chính",
        ///     "statusId": "PENDING",
        ///     "isSuccess": false,
        ///     "orderDetails": [
        ///       {
        ///         "orderDetailId": "od456",
        ///         "productId": "123456",
        ///         "productName": "Sữa tươi nguyên kem",
        ///         "productPrice": 250000,
        ///         "quantity": 2,
        ///         "subTotal": 500000,
        ///         "img": "base64-encoded-image-data"
        ///       }
        ///     ],
        ///     "appliedVouchers": [
        ///       {
        ///         "voucherId": "v789",
        ///         "voucherCode": "SUMMER2023",
        ///         "discountValue": 50000,
        ///         "discountType": "0"
        ///       }
        ///     ]
        ///   }
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (sản phẩm hết hàng):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Sản phẩm 'Sữa tươi nguyên kem' chỉ còn 1 sản phẩm trong kho",
        ///   "statusCode": 400,
        ///   "data": null
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
        /// **Quyền truy cập:**
        /// - Admin: Có thể cập nhật trạng thái của tất cả đơn hàng
        /// - Staff: Có thể cập nhật trạng thái của tất cả đơn hàng
        /// - Khách hàng: Không có quyền truy cập API này
        ///
        /// **Giá trị statusId:**
        /// - **PENDING**: Chờ xác nhận (trạng thái ban đầu khi đơn hàng mới được tạo)
        /// - **CONFIRMED**: Đã xác nhận (đơn hàng đã được xác nhận và đang chuẩn bị)
        /// - **PROCESSING**: Đang xử lý (đơn hàng đang được chuẩn bị)
        /// - **SHIPPING**: Đang giao hàng (đơn hàng đã được giao cho đơn vị vận chuyển)
        /// - **COMPLETED**: Đã hoàn thành (đơn hàng đã được giao thành công)
        /// - **CANCELLED**: Đã hủy (đơn hàng đã bị hủy)
        ///
        /// **Quy trình cập nhật trạng thái:**
        /// - PENDING → CONFIRMED hoặc CANCELLED
        /// - CONFIRMED → PROCESSING hoặc CANCELLED
        /// - PROCESSING → SHIPPING
        /// - SHIPPING → COMPLETED
        /// - Không thể cập nhật đơn hàng đã ở trạng thái COMPLETED hoặc CANCELLED
        ///
        /// **Xử lý của hệ thống:**
        /// - Kiểm tra quy trình: Nếu cập nhật không tuân theo quy trình, trả về lỗi 400
        /// - Khi cập nhật thành COMPLETED:
        ///   + Cập nhật số lượng đã bán (SoldQuantity) của sản phẩm
        ///   + Đánh dấu đơn hàng là thành công (IsSuccess = true)
        ///   + Tăng điểm tích lũy (Coupon) cho khách hàng
        /// - Khi cập nhật thành CANCELLED:
        ///   + Hoàn lại số lượng tồn kho
        ///   + Đánh dấu đơn hàng là không thành công (IsSuccess = false)
        ///
        /// **Ví dụ request:**
        /// ```
        /// PUT /api/Order/123456/status/CONFIRMED
        /// Authorization: Bearer {token}
        /// ```
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Cập nhật trạng thái đơn hàng thành công",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "orderId": "123456",
        ///     "orderNumber": "ORD-20230615-001",
        ///     "statusId": "CONFIRMED",
        ///     "isSuccess": false
        ///   }
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (không tuân theo quy trình):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Không thể cập nhật đơn hàng từ trạng thái PENDING sang SHIPPING. Quy trình hợp lệ: PENDING → CONFIRMED hoặc CANCELLED",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// ```
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
        /// <para><strong>Quyền truy cập:</strong></para>
        /// <list type="bullet">
        /// <item>Chỉ khách hàng đã đăng nhập mới có thể xem lịch sử đơn hàng của mình</item>
        /// <item>Yêu cầu token JWT trong header Authorization</item>
        /// </list>
        ///
        /// <para><strong>Các tham số truy vấn:</strong></para>
        /// <list type="bullet">
        /// <item><c>pageNumber</c>: Số trang (bắt đầu từ 1, mặc định: 1)</item>
        /// <item><c>pageSize</c>: Số lượng đơn hàng mỗi trang (tối đa 50, mặc định: 10)</item>
        /// <item><c>searchTerm</c>: Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)</item>
        /// <item><c>startDate</c>: Ngày bắt đầu (định dạng: yyyy-MM-dd)</item>
        /// <item><c>endDate</c>: Ngày kết thúc (định dạng: yyyy-MM-dd)</item>
        /// <item><c>sortBy</c>: Trường sắp xếp (orderdate, totalprice, ordernumber, status, mặc định: orderdate)</item>
        /// <item><c>sortAscending</c>: Sắp xếp tăng dần (true) hoặc giảm dần (false, mặc định: false)</item>
        /// <item><c>onlySuccessful</c>: Chỉ lấy đơn hàng thành công (mặc định: true)</item>
        /// <item><c>statusId</c>: Lọc theo trạng thái đơn hàng (tùy chọn)</item>
        /// </list>
        ///
        /// <para><strong>Xử lý của hệ thống:</strong></para>
        /// <list type="bullet">
        /// <item>Lấy ID khách hàng từ token JWT</item>
        /// <item>Lọc đơn hàng theo các tiêu chí trong tham số truy vấn</item>
        /// <item>Mặc định chỉ lấy các đơn hàng thành công (IsSuccess = true)</item>
        /// <item>Phân trang kết quả theo pageNumber và pageSize</item>
        /// </list>
        ///
        /// <para><strong>Ví dụ request:</strong></para>
        /// <code>
        /// GET /api/Order/history?pageNumber=1&amp;pageSize=10&amp;startDate=2023-01-01&amp;endDate=2023-12-31&amp;sortBy=orderdate&amp;sortAscending=false
        /// Authorization: Bearer {token}
        /// </code>
        ///
        /// <para><strong>Ví dụ response thành công:</strong></para>
        /// <code>
        /// {
        ///   "success": true,
        ///   "message": "Lấy lịch sử đơn hàng thành công",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "items": [
        ///       {
        ///         "orderId": "123456",
        ///         "orderNumber": "ORD-20230615-001",
        ///         "customerId": "cust123",
        ///         "orderDate": "2023-06-15T10:30:00",
        ///         "shippingAddress": "123 Đường ABC, Quận 1, TP.HCM",
        ///         "shippingFee": 30000,
        ///         "totalPrice": 530000,
        ///         "paymentMethod": "CASH",
        ///         "paymentMethodName": "Tiền mặt",
        ///         "notes": "Giao hàng giờ hành chính",
        ///         "statusId": "COMPLETED",
        ///         "isSuccess": true,
        ///         "orderDetails": [
        ///           {
        ///             "orderDetailId": "od123",
        ///             "productId": "prod456",
        ///             "productName": "Sữa tươi nguyên kem",
        ///             "productPrice": 250000,
        ///             "quantity": 2,
        ///             "subTotal": 500000,
        ///             "img": "base64-encoded-image-data"
        ///           }
        ///         ],
        ///         "appliedVouchers": [
        ///           {
        ///             "voucherId": "v789",
        ///             "voucherCode": "SUMMER2023",
        ///             "discountValue": 50000,
        ///             "discountType": "0"
        ///           }
        ///         ]
        ///       }
        ///     ],
        ///     "metadata": {
        ///       "totalCount": 25,
        ///       "pageSize": 10,
        ///       "currentPage": 1,
        ///       "totalPages": 3,
        ///       "hasNext": true,
        ///       "hasPrevious": false
        ///     }
        ///   }
        /// }
        /// </code>
        ///
        /// <para><strong>Lưu ý cho frontend developers:</strong></para>
        /// <list type="bullet">
        /// <item>Sử dụng thông tin phân trang trong <c>metadata</c> để hiển thị điều hướng trang</item>
        /// <item>Các trường trong <c>orderDetails</c> có thể được sử dụng để hiển thị thông tin sản phẩm</item>
        /// <item>Trường <c>img</c> chứa dữ liệu hình ảnh dạng base64, có thể hiển thị trực tiếp trong thẻ img</item>
        /// <item>Trường <c>statusId</c> có thể được sử dụng để hiển thị trạng thái đơn hàng với màu sắc khác nhau</item>
        /// </list>
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
