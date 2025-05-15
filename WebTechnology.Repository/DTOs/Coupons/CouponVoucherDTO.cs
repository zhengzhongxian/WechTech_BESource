using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;

namespace WebTechnology.Repository.DTOs.Coupons
{
    /// <summary>
    /// DTO cho việc hiển thị thông tin voucher có thể đổi bằng điểm coupon
    /// </summary>
    public class CouponVoucherDTO
    {
        /// <summary>
        /// ID của voucher
        /// </summary>
        public string VoucherId { get; set; }

        /// <summary>
        /// Mã voucher
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Giá trị giảm giá
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Loại giảm giá (Percentage hoặc FixedAmount)
        /// </summary>
        public DiscountType DiscountType { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Giá trị đơn hàng tối thiểu
        /// </summary>
        public decimal? MinOrder { get; set; }

        /// <summary>
        /// Giá trị giảm tối đa
        /// </summary>
        public decimal? MaxDiscount { get; set; }

        /// <summary>
        /// Số điểm coupon cần để đổi voucher này
        /// </summary>
        public int PointsRequired { get; set; }

        /// <summary>
        /// Mô tả về voucher
        /// </summary>
        public string Description { get; set; }
    }
}
