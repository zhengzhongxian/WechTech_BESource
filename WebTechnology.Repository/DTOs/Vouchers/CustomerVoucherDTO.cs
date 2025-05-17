using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;

namespace WebTechnology.Repository.DTOs.Vouchers
{
    /// <summary>
    /// DTO cho việc hiển thị thông tin voucher của khách hàng
    /// </summary>
    public class CustomerVoucherDTO
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
        /// Loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
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
        /// Giá trị giảm tối đa (chỉ áp dụng cho giảm giá theo phần trăm)
        /// </summary>
        public decimal? MaxDiscount { get; set; }

        /// <summary>
        /// Ngày tạo voucher
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Thông tin bổ sung về voucher
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ngày khách hàng nhận được voucher
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Kiểm tra xem voucher đã hết hạn hay chưa
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Kiểm tra xem voucher còn lượt sử dụng hay không
        /// </summary>
        public bool HasAvailableUsage { get; set; }
    }
}
