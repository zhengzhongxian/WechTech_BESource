﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class CustomerDetailDTO
    {
        // Thông tin từ bảng User
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        // Thông tin từ bảng Customer
        public string CustomerId { get; set; }
        public string Surname { get; set; }
        public string Middlename { get; set; }
        public string Firstname { get; set; }
        public string FullName => $"{Surname} {Middlename} {Firstname}".Trim();
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string AvatarPublicId { get; set; }
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
    }
}
