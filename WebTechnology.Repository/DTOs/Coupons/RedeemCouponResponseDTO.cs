using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Coupons
{
    /// <summary>
    /// DTO cho kết quả đổi điểm coupon lấy voucher
    /// </summary>
    public class RedeemCouponResponseDTO
    {
        /// <summary>
        /// ID của voucher đã đổi
        /// </summary>
        public string VoucherId { get; set; }

        /// <summary>
        /// Mã voucher đã đổi
        /// </summary>
        public string VoucherCode { get; set; }

        /// <summary>
        /// Số điểm coupon đã sử dụng
        /// </summary>
        public int PointsUsed { get; set; }

        /// <summary>
        /// Số điểm coupon còn lại
        /// </summary>
        public int RemainingPoints { get; set; }

        /// <summary>
        /// Thông tin về voucher đã đổi
        /// </summary>
        public string VoucherInfo { get; set; }

        /// <summary>
        /// Ngày hết hạn của voucher
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}
