using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Coupons
{
    /// <summary>
    /// DTO cho việc đổi điểm coupon lấy voucher
    /// </summary>
    public class RedeemCouponDTO
    {
        /// <summary>
        /// ID của voucher muốn đổi
        /// </summary>
        [Required(ErrorMessage = "ID voucher không được để trống")]
        public string VoucherId { get; set; }
    }
}
