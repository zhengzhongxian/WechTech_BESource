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
    }
}
