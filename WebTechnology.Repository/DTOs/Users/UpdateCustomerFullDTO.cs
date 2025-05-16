﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class UpdateCustomerFullDTO
    {
        // Thông tin cơ bản từ User
        [Required(ErrorMessage = "Username không được để trống")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        // Mật khẩu (có thể null nếu không muốn thay đổi)
        public string? Password { get; set; }

        // Thông tin từ Customer
        public string? Surname { get; set; }
        public string? Middlename { get; set; }
        public string? Firstname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        /// <summary>
        /// Ảnh đại diện dưới dạng base64
        /// </summary>
        public string? AvatarBase64 { get; set; }

        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }

        // Coupon - chỉ Admin mới có thể cập nhật
        public int? Coupoun { get; set; }

        // Thông tin trạng thái
        public bool IsActive { get; set; } = true;
        public string? StatusId { get; set; }
    }
}
