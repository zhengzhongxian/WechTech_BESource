﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class UpdateAdminStaffDTO
    {
        // Thông tin cơ bản
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        // Mật khẩu (có thể null nếu không muốn thay đổi)
        public string? Password { get; set; }

        // Không cần thông tin cá nhân vì Admin/Staff chỉ có thông tin trong bảng User

        // Thông tin trạng thái
        public bool IsActive { get; set; } = true;
        public string? StatusId { get; set; }

        // Thông tin vai trò
        public string? RoleId { get; set; }
    }
}
