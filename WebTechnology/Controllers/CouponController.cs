using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Coupons;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    /// <summary>
    /// API Controller để quản lý điểm coupon và đổi điểm lấy voucher
    /// </summary>
    /// <remarks>
    /// Controller này cung cấp các API để:
    /// - Đổi điểm coupon lấy voucher
    /// - Xem lịch sử đổi điểm
    /// - Xem số điểm coupon hiện tại
    ///
    /// Điểm coupon được tích lũy khi khách hàng hoàn thành đơn hàng và có thể được sử dụng để đổi lấy các voucher giảm giá.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly ITokenService _tokenService;

        public CouponController(ICouponService couponService, ITokenService tokenService)
        {
            _couponService = couponService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Lấy danh sách các voucher có thể đổi bằng điểm coupon
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các voucher mà khách hàng có thể đổi bằng điểm coupon.
        /// Mỗi voucher sẽ có thông tin về số điểm cần thiết để đổi (PointsRequired).
        ///
        /// Endpoint này không yêu cầu xác thực, bất kỳ ai cũng có thể xem danh sách voucher có sẵn.
        /// </remarks>
        /// <returns>
        /// Danh sách các voucher có thể đổi, bao gồm:
        /// - VoucherId: ID của voucher
        /// - Code: Mã voucher gốc
        /// - DiscountValue: Giá trị giảm giá
        /// - DiscountType: Loại giảm giá (Percentage hoặc FixedAmount)
        /// - StartDate: Ngày bắt đầu
        /// - EndDate: Ngày kết thúc
        /// - MinOrder: Giá trị đơn hàng tối thiểu
        /// - MaxDiscount: Giá trị giảm tối đa
        /// - PointsRequired: Số điểm coupon cần để đổi voucher này
        /// - Description: Mô tả về voucher
        /// </returns>
        /// <response code="200">Trả về danh sách voucher thành công</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("available-vouchers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableVouchers()
        {
            var result = await _couponService.GetAvailableVouchersAsync();
            return StatusCode((int)result.StatusCode, result);
        }


        /// <summary>
        /// Đổi điểm coupon lấy voucher
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng đổi điểm coupon của họ để lấy voucher giảm giá.
        /// Khách hàng cần cung cấp ID của voucher muốn đổi.
        ///
        /// Hệ thống sẽ tự động:
        /// - Kiểm tra xem khách hàng có đủ điểm để đổi voucher không
        /// - Kiểm tra xem voucher có còn trong giới hạn sử dụng không
        /// - Tạo một mã voucher duy nhất cho khách hàng
        /// - Trừ điểm coupon của khách hàng
        /// - Tăng số lượng đã sử dụng của voucher gốc
        ///
        /// Endpoint này yêu cầu xác thực với quyền "CustomerOnly".
        /// </remarks>
        /// <param name="redeemDto">Thông tin đổi điểm, bao gồm VoucherId</param>
        /// <returns>
        /// Thông tin về voucher đã đổi, bao gồm:
        /// - VoucherId: ID của voucher đã đổi
        /// - VoucherCode: Mã voucher duy nhất đã đổi
        /// - PointsUsed: Số điểm coupon đã sử dụng
        /// - RemainingPoints: Số điểm coupon còn lại
        /// - VoucherInfo: Thông tin về voucher đã đổi
        /// - ExpiryDate: Ngày hết hạn của voucher
        /// </returns>
        /// <response code="200">Đổi điểm thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc không đủ điểm</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy voucher</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPost("redeem")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> RedeemCoupon([FromBody] RedeemCouponDTO redeemDto)
        {
            // Lấy customerId từ token
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var customerId = _tokenService.GetUserIdFromToken(token);

            // Gọi service để đổi điểm
            var result = await _couponService.RedeemCouponAsync(redeemDto, customerId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Lấy lịch sử đổi điểm coupon của khách hàng
        /// </summary>
        /// <remarks>
        /// API này trả về lịch sử đổi điểm coupon của một khách hàng cụ thể.
        /// Lịch sử bao gồm tất cả các voucher mà khách hàng đã đổi bằng điểm coupon.
        ///
        /// Quy tắc bảo mật:
        /// - Khách hàng chỉ có thể xem lịch sử đổi điểm của chính họ
        /// - Admin có thể xem lịch sử đổi điểm của bất kỳ khách hàng nào
        ///
        /// Endpoint này yêu cầu xác thực với quyền "AdminOrCustomer".
        /// </remarks>
        /// <param name="customerId">ID của khách hàng cần xem lịch sử</param>
        /// <returns>
        /// Danh sách các voucher đã đổi, mỗi voucher bao gồm:
        /// - VoucherId: ID của voucher đã đổi
        /// - VoucherCode: Mã voucher duy nhất đã đổi
        /// - PointsUsed: Số điểm coupon đã sử dụng
        /// - VoucherInfo: Thông tin về voucher đã đổi
        /// - ExpiryDate: Ngày hết hạn của voucher
        /// </returns>
        /// <response code="200">Lấy lịch sử đổi điểm thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không có quyền xem lịch sử đổi điểm của người khác</response>
        /// <response code="404">Không tìm thấy khách hàng</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("redemption-history/{customerId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetRedemptionHistory(string customerId)
        {
            // Lấy customerId từ token
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var tokenCustomerId = _tokenService.GetUserIdFromToken(token);
            var role = _tokenService.GetRoleFromToken(token);

            // Kiểm tra xem người dùng có quyền xem lịch sử đổi điểm của người khác không
            if (role != RoleType.Admin.ToRoleIdString() && tokenCustomerId != customerId)
            {
                return StatusCode(403, new { Success = false, Message = "Bạn không có quyền xem lịch sử đổi điểm của người khác" });
            }

            var result = await _couponService.GetRedemptionHistoryAsync(customerId);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Lấy số điểm coupon hiện tại của khách hàng
        /// </summary>
        /// <remarks>
        /// API này trả về số điểm coupon hiện tại của một khách hàng cụ thể.
        /// Điểm coupon được tích lũy khi khách hàng hoàn thành đơn hàng và có thể được sử dụng để đổi lấy voucher.
        ///
        /// Quy tắc bảo mật:
        /// - Khách hàng chỉ có thể xem số điểm của chính họ
        /// - Admin có thể xem số điểm của bất kỳ khách hàng nào
        ///
        /// Endpoint này yêu cầu xác thực với quyền "AdminOrCustomer".
        /// </remarks>
        /// <param name="customerId">ID của khách hàng cần xem số điểm</param>
        /// <returns>
        /// Số điểm coupon hiện tại của khách hàng
        /// </returns>
        /// <example>
        /// Ví dụ response thành công:
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Lấy số điểm coupon thành công",
        ///   "statusCode": 200,
        ///   "data": 150
        /// }
        /// ```
        /// </example>
        /// <response code="200">Lấy số điểm coupon thành công</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không có quyền xem số điểm của người khác</response>
        /// <response code="404">Không tìm thấy khách hàng</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("current-points/{customerId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetCurrentPoints(string? customerId)
        {
            // Lấy customerId từ token
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var tokenCustomerId = _tokenService.GetUserIdFromToken(token);
            var role = _tokenService.GetRoleFromToken(token);

            // Kiểm tra xem người dùng có quyền xem số điểm của người khác không
            if (role != RoleType.Admin.ToRoleIdString() && tokenCustomerId != customerId)
            {
                return StatusCode(403, new { Success = false, Message = "Bạn không có quyền xem số điểm của người khác" });
            }

            var result = await _couponService.GetCurrentPointsAsync(customerId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
