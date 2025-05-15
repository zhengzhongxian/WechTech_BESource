﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Cart
{
    /// <summary>
    /// Mô hình yêu cầu để lấy danh sách sản phẩm trong giỏ hàng có phân trang
    /// </summary>
    public class CartItemQueryRequest
    {
        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        [Range(1, int.MaxValue)]
        [Required]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng sản phẩm mỗi trang (tối đa 50)
        /// </summary>
        [Range(1, 50)]
        [Required(ErrorMessage = "PageSize không được vượt 50")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Từ khóa tìm kiếm (tìm trong tên sản phẩm)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sắp xếp theo (CreatedAt, ProductName, Price)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </summary>
        public bool SortAscending { get; set; } = false;
    }
}
