using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ thanh toán Payos
    /// </summary>
    public interface IPayosService
    {
        /// <summary>
        /// Tạo link thanh toán Payos
        /// </summary>
        /// <param name="request">Thông tin thanh toán</param>
        /// <returns>Thông tin link thanh toán</returns>
        Task<ServiceResponse<PayosPaymentData>> CreatePaymentLinkAsync(PayosCreatePaymentLinkRequest request);

        /// <summary>
        /// Xử lý webhook từ Payos
        /// </summary>
        /// <param name="webhookRequest">Dữ liệu webhook</param>
        /// <returns>Kết quả xử lý</returns>
        Task<ServiceResponse<bool>> ProcessWebhookAsync(PayosWebhookRequest webhookRequest);

        /// <summary>
        /// Kiểm tra trạng thái thanh toán
        /// </summary>
        /// <param name="paymentLinkId">ID giao dịch trong hệ thống Payos</param>
        /// <returns>Thông tin trạng thái thanh toán</returns>
        Task<ServiceResponse<string>> CheckPaymentStatusAsync(string paymentLinkId);

        /// <summary>
        /// Xác nhận webhook URL với Payos
        /// </summary>
        /// <param name="webhookUrl">URL webhook cần xác nhận</param>
        /// <returns>Kết quả xác nhận</returns>
        Task<ServiceResponse<bool>> ConfirmWebhookAsync(string webhookUrl);
    }
}
