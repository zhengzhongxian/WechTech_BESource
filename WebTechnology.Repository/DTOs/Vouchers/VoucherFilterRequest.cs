using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Vouchers
{
    /// <summary>
    /// DTO cho việc lọc danh sách voucher
    /// </summary>
    public class VoucherFilterRequest
    {
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
    }
}
