﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class UpdateCustomerDetailDTO
    {
        // Thông tin từ bảng User
        [StringLength(100, ErrorMessage = "Username không được vượt quá 100 ký tự")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(64, ErrorMessage = "StatusId không được vượt quá 64 ký tự")]
        public string StatusId { get; set; }

        [StringLength(64, ErrorMessage = "RoleId không được vượt quá 64 ký tự")]
        public string RoleId { get; set; }

        // Thông tin từ bảng Customer
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        public string Surname { get; set; }

        [StringLength(50, ErrorMessage = "Tên đệm không được vượt quá 50 ký tự")]
        public string Middlename { get; set; }

        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string Firstname { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ được chứa các chữ số")]
        public string PhoneNumber { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string Address { get; set; }

        //[StringLength(255, ErrorMessage = "Đường dẫn avatar không được vượt quá 255 ký tự")]
        //public string Avatar { get; set; }

        /// <summary>
        /// Ảnh đại diện dưới dạng base64
        /// </summary>
        public string? AvatarBase64 { get; set; }

        /// <summary>
        /// Public ID của ảnh trên Cloudinary
        /// </summary>
        //public string AvatarPublicId { get; set; }

        public DateTime? Dob { get; set; }

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự")]
        public string Gender { get; set; }
    }
}
