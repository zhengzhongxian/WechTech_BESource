using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.CoreHelpers.ValidationCustom;

namespace WebTechnology.Service.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Yêu cầu tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
        public string UserName {  get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StrongPassword(ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
