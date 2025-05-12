using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class UserQueryRequest
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
        /// Từ khóa tìm kiếm (tìm theo username hoặc email)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sắp xếp theo trường nào (Username, Email, CreatedAt)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sắp xếp tăng dần hay giảm dần
        /// </summary>
        public bool SortAscending { get; set; } = false;

        /// <summary>
        /// Lọc theo trạng thái người dùng
        /// </summary>
        public string? StatusId { get; set; }

        /// <summary>
        /// Lọc theo vai trò người dùng
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Chỉ hiển thị người dùng đang hoạt động
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
