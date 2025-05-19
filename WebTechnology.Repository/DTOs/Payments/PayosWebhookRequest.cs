using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Payments
{
    /// <summary>
    /// Request DTO từ webhook Payos
    /// </summary>
    public class PayosWebhookRequest
    {
        /// <summary>
        /// Dữ liệu webhook
        /// </summary>
        [JsonPropertyName("data")]
        public PayosWebhookData Data { get; set; }

        /// <summary>
        /// Chữ ký xác thực
        /// </summary>
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }

    /// <summary>
    /// Dữ liệu webhook từ Payos
    /// </summary>
    public class PayosWebhookData
    {
        /// <summary>
        /// ID giao dịch trong hệ thống Payos
        /// </summary>
        [JsonPropertyName("paymentLinkId")]
        public string PaymentLinkId { get; set; }

        /// <summary>
        /// Mã đơn hàng trong hệ thống của bạn
        /// </summary>
        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// Trạng thái thanh toán
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Số tiền thanh toán (VND)
        /// </summary>
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// Mô tả đơn hàng
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Thời gian thanh toán
        /// </summary>
        [JsonPropertyName("transactionTime")]
        public long TransactionTime { get; set; }
    }
}
