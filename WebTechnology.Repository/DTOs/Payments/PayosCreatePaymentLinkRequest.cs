using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Payments
{
    /// <summary>
    /// Request DTO để tạo link thanh toán Payos
    /// </summary>
    public class PayosCreatePaymentLinkRequest
    {
        /// <summary>
        /// ID đơn hàng trong hệ thống của bạn
        /// </summary>
        [Required]
        public string OrderId { get; set; }

        /// <summary>
        /// Số tiền thanh toán (VND)
        /// </summary>
        [JsonIgnore]
        public int? Amount { get; set; }

        /// <summary>
        /// Mô tả đơn hàng
        /// </summary>
        [JsonIgnore]
        public string? Description { get; set; }

        /// <summary>
        /// URL callback khi thanh toán thành công
        /// </summary>
        [Required]
        [Url]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// URL callback khi thanh toán thất bại
        /// </summary>
        [Url]
        public string CancelUrl { get; set; }

        /// <summary>
        /// Thông tin khách hàng
        /// </summary>
        [JsonIgnore]
        public PayosCustomerInfo? CustomerInfo { get; set; }
    }

    /// <summary>
    /// Thông tin khách hàng cho Payos
    /// </summary>
    public class PayosCustomerInfo
    {
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email khách hàng
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string Phone { get; set; }
    }
}
