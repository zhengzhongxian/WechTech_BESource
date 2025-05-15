using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebTechnology.API;

namespace WebTechnology.Repository.DTOs.Vouchers
{
    public class CreateVoucherDTO
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(50, ErrorMessage = "Mã giảm giá không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải là số dương")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn sử dụng phải là số dương")]
        public int? UsageLimit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu phải là số dương")]
        public decimal? MinOrder { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm tối đa phải là số dương")]
        public decimal? MaxDiscount { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        public bool IsRoot { get; set; } = true;
        public int? Point { get; set; } = 0;
        public string? Metadata { get; set; }
    }
}