﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    /// <summary>
    /// Mô hình yêu cầu để lấy danh sách đơn hàng có phân trang
    /// </summary>
    public class OrderQueryRequest
    {
        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        [Range(1, int.MaxValue)]
        [Required]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng đơn hàng mỗi trang (tối đa 50)
        /// </summary>
        [Range(1, 50)]
        [Required(ErrorMessage = "PageSize không được vượt 50")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// ID của khách hàng cần lọc
        /// </summary>
        public string? CustomerId { get; set; }

        /// <summary>
        /// ID trạng thái đơn hàng (PENDING, CONFIRMED, SHIPPING, COMPLETED, CANCELLED)
        /// </summary>
        public string? StatusId { get; set; }

        /// <summary>
        /// Từ khóa tìm kiếm (tìm trong mã đơn hàng, địa chỉ giao hàng, ghi chú)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Ngày bắt đầu (định dạng: yyyy-MM-dd)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc (định dạng: yyyy-MM-dd)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Trường sắp xếp (orderdate, totalprice, ordernumber, status)
        /// </summary>
        public string? SortBy { get; set; } = "OrderDate";

        /// <summary>
        /// Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </summary>
        public bool SortAscending { get; set; } = false;
    }
}
