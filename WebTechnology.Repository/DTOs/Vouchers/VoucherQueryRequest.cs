using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;

namespace WebTechnology.Repository.DTOs.Vouchers
{
    public class VoucherQueryRequest
    {
        /// <summary>
        /// Số trang hiện tại (bắt đầu từ 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng bản ghi trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Từ khóa tìm kiếm (tìm theo code)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sắp xếp theo trường nào (CreatedAt, Code, DiscountValue)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sắp xếp tăng dần hay giảm dần
        /// </summary>
        public bool SortAscending { get; set; } = false;

        /// <summary>
        /// Lọc theo trạng thái hoạt động
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Lọc theo loại voucher (phần trăm hoặc giá trị cố định)
        /// </summary>
        public DiscountType? DiscountType { get; set; }

        /// <summary>
        /// Lọc theo voucher gốc (true) hoặc voucher con (false)
        /// </summary>
        public bool? IsRoot { get; set; }

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
    }
}
