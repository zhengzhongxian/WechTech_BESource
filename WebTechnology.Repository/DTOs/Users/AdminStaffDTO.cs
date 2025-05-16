﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class AdminStaffDTO
    {
        // Thông tin từ bảng User
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Mật khẩu đã được băm, không thể giải mã
        /// </summary>
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        // Các trường bổ sung có thể được thêm vào trong tương lai nếu cần
    }
}
