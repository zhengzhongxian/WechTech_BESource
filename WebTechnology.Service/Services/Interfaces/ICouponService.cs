using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Coupons;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ICouponService
    {
        /// <summary>
        /// Lấy danh sách các voucher có thể đổi bằng điểm coupon
        /// </summary>
        /// <returns>Danh sách các voucher có thể đổi</returns>
        Task<ServiceResponse<List<CouponVoucherDTO>>> GetAvailableVouchersAsync();

        /// <summary>
        /// Đổi điểm coupon lấy voucher
        /// </summary>
        /// <param name="redeemDto">Thông tin đổi điểm</param>
        /// <param name="customerId">ID của khách hàng (lấy từ token)</param>
        /// <returns>Kết quả đổi điểm</returns>
        Task<ServiceResponse<RedeemCouponResponseDTO>> RedeemCouponAsync(RedeemCouponDTO redeemDto, string customerId);

        /// <summary>
        /// Lấy lịch sử đổi điểm coupon của khách hàng
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <returns>Lịch sử đổi điểm</returns>
        Task<ServiceResponse<List<RedeemCouponResponseDTO>>> GetRedemptionHistoryAsync(string customerId);

        /// <summary>
        /// Lấy số điểm coupon hiện tại của khách hàng
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <returns>Số điểm coupon hiện tại</returns>
        Task<ServiceResponse<int>> GetCurrentPointsAsync(string customerId);
    }
}
