using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string recipientEmail, string recipientName, string otpCode);

        /// <summary>
        /// Gửi email đặt lại mật khẩu
        /// </summary>
        /// <param name="recipientEmail">Email người nhận</param>
        /// <param name="recipientName">Tên người nhận</param>
        /// <param name="resetToken">Token đặt lại mật khẩu</param>
        /// <param name="resetUrl">URL đặt lại mật khẩu</param>
        Task SendPasswordResetEmailAsync(string recipientEmail, string recipientName, string resetToken, string resetUrl);
    }
}
