using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.CoreHelpers.ValidationCustom;

namespace WebTechnology.Repository.DTOs.Users
{
    public class RegistrationRequestDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StrongPassword(ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP phải là 6 chữ số")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string Surname { get; set; }

        public string Middlename { get; set; }

        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime Dob { get; set; }

        [Gender(ErrorMessage = "Giới tính không hợp lệ! Chỉ chấp nhận 'Nam' hoặc 'Nữ'.")]
        public string Gender { get; set; }
    }
}
