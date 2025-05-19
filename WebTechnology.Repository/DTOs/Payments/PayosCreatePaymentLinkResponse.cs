using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Payments
{
    /// <summary>
    /// Response DTO từ Payos khi tạo link thanh toán
    /// </summary>
    public class PayosCreatePaymentLinkResponse
    {
        /// <summary>
        /// Mã trạng thái
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        [JsonPropertyName("data")]
        public PayosPaymentData Data { get; set; }
    }

    /// <summary>
    /// Dữ liệu thanh toán từ Payos
    /// </summary>
    public class PayosPaymentData
    {
        /// <summary>
        /// ID giao dịch trong hệ thống Payos
        /// </summary>
        [JsonPropertyName("id")]
        public string PaymentLinkId { get; set; }

        /// <summary>
        /// URL thanh toán
        /// </summary>
        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; }

        /// <summary>
        /// Mã QR thanh toán
        /// </summary>
        [JsonPropertyName("qrCode")]
        public string QrCode { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        [JsonPropertyName("expiredAt")]
        public long ExpiredAt { get; set; }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
