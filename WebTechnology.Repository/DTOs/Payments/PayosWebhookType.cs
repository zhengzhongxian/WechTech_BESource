using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Net.payOS.Types;
using Newtonsoft.Json.Linq;

namespace WebTechnology.Repository.DTOs.Payments
{
    /// <summary>
    /// Lớp chuyển đổi giữa PayosWebhookRequest và WebhookType của thư viện PayOS
    /// </summary>
    public class PayosWebhookType
    {
        /// <summary>
        /// Chuyển đổi từ PayosWebhookRequest sang WebhookType
        /// </summary>
        /// <param name="request">PayosWebhookRequest</param>
        /// <returns>WebhookType</returns>
        public static Net.payOS.Types.WebhookType FromPayosWebhookRequest(PayosWebhookRequest request)
        {
            if (request == null)
                return null;

            // Chuyển đổi orderCode từ string sang long
            long orderCodeLong = 0;
            if (!string.IsNullOrEmpty(request.Data.OrderCode))
            {
                long.TryParse(request.Data.OrderCode, out orderCodeLong);
            }

            // Tạo WebhookData từ PayosWebhookData
            // Sử dụng JObject để tạo đối tượng WebhookData
            var dataObj = new JObject();
            dataObj["orderCode"] = orderCodeLong;
            dataObj["amount"] = request.Data.Amount;
            dataObj["description"] = request.Data.Description;
            dataObj["paymentLinkId"] = request.Data.PaymentLinkId;
            dataObj["transactionTime"] = request.Data.TransactionTime;

            // Chuyển đổi JObject thành WebhookData
            var webhookData = dataObj.ToObject<Net.payOS.Types.WebhookData>();

            // Tạo WebhookType
            // Sử dụng JObject để tạo đối tượng WebhookType
            var webhookTypeObj = new JObject();
            webhookTypeObj["data"] = JObject.FromObject(webhookData);
            webhookTypeObj["signature"] = request.Signature;

            // Chuyển đổi JObject thành WebhookType
            return webhookTypeObj.ToObject<Net.payOS.Types.WebhookType>();
        }
    }
}
