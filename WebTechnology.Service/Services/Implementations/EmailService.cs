using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;
using WebTechnology.Service.CoreHelpers;
using Microsoft.Extensions.Configuration;

namespace WebTechnology.Service.Services.Implementationns
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSetting;
        private readonly SmtpClient _smtpClient;
        public EmailService(IOptions<EmailSetting> options)
        {
            _emailSetting = options.Value;
            _smtpClient = new SmtpClient(_emailSetting.Smtp.Host)
            {
                Port = _emailSetting.Smtp.Port,
                Credentials = new NetworkCredential(_emailSetting.Smtp.EmailAddress, _emailSetting.Smtp.Password),
                EnableSsl = _emailSetting.Smtp.EnableSsl
            };
        }
        public async Task SendOtpEmailAsync(string recipientEmail, string recipientName, string otpCode)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSetting.FromEmailAddress, _emailSetting.FromDisplayName),
                Subject = "Mã OTP xác thực",
                Body = MailBody.GetOtpEmailBody(otpCode, recipientName),
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipientEmail);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
