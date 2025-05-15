using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Vouchers
{
    /// <summary>
    /// DTO cho việc truy vấn danh sách voucher của khách hàng
    /// </summary>
    public class CustomerVoucherQueryRequest
    {
        /// <summary>
        /// ID của khách hàng
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng voucher mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Từ khóa tìm kiếm (tìm trong mã voucher)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Lọc theo trạng thái hoạt động
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Lọc theo loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
        /// </summary>
        public int? DiscountType { get; set; }

        /// <summary>
        /// Lọc theo ngày bắt đầu từ
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Lọc theo ngày bắt đầu đến
        /// </summary>
        public DateTime? StartDateTo { get; set; }

        /// <summary>
        /// Lọc theo ngày kết thúc từ
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// Lọc theo ngày kết thúc đến
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        /// <summary>
        /// Trường sắp xếp (code, discountValue, startDate, endDate, createdAt)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </summary>
        public bool SortAscending { get; set; } = false;
    }
}
