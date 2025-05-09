using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.UserStatuses
{
    public class CreateUserStatusDTO
    {
        [Required(ErrorMessage = "Mã trạng thái là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã trạng thái không được vượt quá 50 ký tự")]
        public string StatusId { get; set; }

        [Required(ErrorMessage = "Tên trạng thái là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên trạng thái không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string? Description { get; set; }
    }
}