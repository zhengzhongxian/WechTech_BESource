using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WebTechnology.Service.CoreHelpers
{
    public static class MailBody
    {
        public static string GetOtpEmailBody(string otpCode, string recipientName)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>OTP Request</title>
    <style>
      body,
      html {{
        height: 100%;
        margin: 0;
        padding: 0;
      }}
      body {{
        display: flex;
        align-items: center;
        justify-content: center;
        font-family: ""Manrope"", sans-serif;
        padding: 20px;
        box-sizing: border-box;
        background-color: #f5f5f5;
      }}
      .container {{
        width: 100%;
        max-width: 500px;
        overflow: hidden;
        border-radius: 12px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
      }}
      .header {{
        background: linear-gradient(to right, #2b7fff, #154ce6);
        color: white;
        display: flex;
        justify-content: center !important;
        align-items: center;
        padding: 18px;
      }}
      .milk-icon {{
        font-size: 1.6rem;
      }}
      .logo {{
        font-size: 1.3rem;
        font-weight: 800;
        letter-spacing: 0.025em;
        margin: 0;
        font-family: Arial, sans-serif;
      }}
      .logo-primary {{
        color: #ffffff;
      }}
      .logo-secondary {{
        color: #e0e7ff;
      }}
      .main {{
        padding: 30px;
        text-align: center;
        background: white;
      }}
      .otp-title {{
        font-size: 1.2rem;
        color: #1e293b;
        margin-bottom: 18px;
        font-family: ""Inter"", sans-serif;
        font-weight: 600;
        letter-spacing: -0.01em;
      }}
      strong {{
        color: #2b7fff;
      }}
      .otp {{
        margin: 0 auto;
        border-radius: 10px;
        border: 3px solid #2b7fff;
        width: 100%;
        max-width: 130px;
        text-align: center;
        color: #2b7fff;
        font-weight: 700;
        font-size: 1.6rem;
        background-color: #f1f5f9;
        padding-top: 10px;
        padding-bottom: 10px;
        margin-bottom: 24px;
        font-family: ""Inter"", sans-serif;
      }}
      .otp-instruction {{
        font-size: 0.93rem;
        color: #64748b;
        line-height: 1.6;
        font-family: ""Manrope"", sans-serif;
        font-weight: 500;
      }}
      .otp-warning {{
        font-size: 0.85rem;
        color: #ff4757;
        margin-top: 18px;
        font-weight: 600;
        font-family: ""Inter"", sans-serif;
      }}
      .footer {{
        padding: 16px;
        text-align: center;
        font-size: 0.8rem;
        color: #94a3b8;
        border-top: 1px solid #e2e8f0;
        background-color: #f8fafc;
      }}
      .thank-you {{
        font-size: 0.9rem;
        color: #475569;
        margin-top: 18px;
        font-style: italic;
      }}
      .copyright {{
        font-size: 0.75rem;
        color: #94a3b8;
        margin-top: 8px;
      }}
      .otp-hello {{
        text-align: left;
        font-size: 0.93rem;
        margin-bottom: 5px;
      }}
      .otp-hello span {{
        color: black;
        font-weight: 900;
      }}
      .notcenter {{
        text-align: left;
      }}
      @media (max-width: 600px) {{
        .container {{
          max-width: 100%;
        }}
        .header {{
          padding: 15px;
        }}
        .milk-icon {{
          font-size: 1.3rem;
        }}
        .logo {{
          font-size: 1.1rem;
        }}
        .main {{
          padding: 24px 18px;
        }}
        .otp {{
          font-size: 1.4rem;
          padding: 10px;
          max-width: 140px;
        }}
        .otp-title {{
          font-size: 1.1rem;
        }}
        .thank-you {{
          margin-top: 18px;
        }}
      }}
      @media (max-width: 400px) {{
        .logo {{
          font-size: 1rem;
        }}
        .otp-title {{
          font-size: 1rem;
        }}
        .otp-instruction {{
          font-size: 0.88rem;
        }}
      }}
    </style>
  </head>
  <body>
    <div class=""container"">
      <form>
        <div class=""header"" style=""justify-content: center; align-items: center;"">
          <span class=""milk-icon"">🥛</span>
          <h1 class=""logo"">
            <span class=""logo-primary"">Milk</span>
            <span class=""logo-secondary"">Store</span>
          </h1>
        </div>
        <div class=""main"">
          <div class=""otp-hello"">Thân gửi <span>{recipientName}</span>,</div>
          <div class=""otp-title notcenter"">Mã xác thực OTP của bạn:</div>
          <div class=""otp"">{otpCode}</div>
          <div class=""otp-instruction notcenter"">
            Vui lòng nhập mã này tại trang xác thực để hoàn tất đăng nhập.
          </div>
          <div class=""otp-instruction notcenter"">
            Mã có hiệu lực trong vòng <strong>5 phút</strong>.
          </div>
          <div class=""otp-warning"">⚠ Không chia sẻ mã này với bất kỳ ai</div>
          <div class=""thank-you"">
            Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!
          </div>
        </div>
        <div class=""footer"">
          <div class=""copyright"">© 2025 MilkStore. All rights reserved.</div>
        </div>
      </form>
    </div>
  </body>
</html>
";
        }
    }
}