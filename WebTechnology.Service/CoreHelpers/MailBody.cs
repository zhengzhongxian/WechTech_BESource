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
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
    <title>Mã xác thực từ MilkStore</title>
    <style>
        /* Reset styles */
        body, html {{
            margin: 0;
            padding: 0;
            font-family: 'Roboto', 'Google Sans', Arial, sans-serif;
            font-size: 14px;
            line-height: 1.5;
            color: #202124;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }}

        * {{
            box-sizing: border-box;
        }}

        /* Email container */
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
        }}

        /* Header */
        .header {{
            padding: 15px 0;
            text-align: center;
            border-bottom: 1px solid #f1f3f4;
            display: flex;
            background: linear-gradient(135deg, #0070f3 0%, #00c6ff 100%);
            justify-content: center;
            border-top-right-radius: 10px;
            border-top-left-radius: 10px;
        }}

        .logo {{
            display: inline-block;
        }}

        .logo img {{
            height: 40px;
            width: auto;
        }}

        /* Content */
        .content {{
            padding: 40px 24px;
        }}

        .greeting {{
            font-size: 16px;
            margin-bottom: 16px;
            color: #202124;
        }}

        .message {{
            font-size: 14px;
            margin-bottom: 24px;
            color: #5f6368;
        }}

        .otp-container {{
            margin: 32px 0;
            text-align: center;
        }}

        .otp-code {{
            display: inline-block;
            font-family: 'Roboto Mono', monospace;
            font-size: 32px;
            font-weight: 500;
            letter-spacing: 4px;
            color: #1a73e8;
            background-color: #f8f9fa;
            padding: 16px 32px;
            border-radius: 8px;
            border: 1px solid #dadce0;
        }}

        .expiry-note {{
            margin-top: 16px;
            font-size: 14px;
            color: #5f6368;
        }}

        .warning {{
            margin-top: 24px;
            padding: 12px 16px;
            background-color: #fef7e0;
            border-left: 4px solid #fbbc04;
            color: #3c4043;
            font-size: 13px;
        }}

        .warning-icon {{
            color: #ea8600;
            margin-right: 8px;
        }}

        /* Footer */
        .footer {{
            padding: 24px;
            text-align: center;
            color: #5f6368;
            font-size: 12px;
            border-top: 1px solid #f1f3f4;
            background-color: #f8f9fa;
        }}

        .footer p {{
            margin: 4px 0;
        }}

        .divider {{
            height: 1px;
            background-color: #f1f3f4;
            margin: 24px 0;
        }}

        /* Responsive */
        @media screen and (max-width: 480px) {{
            .content {{
                padding: 24px 16px;
            }}

            .otp-code {{
                font-size: 28px;
                padding: 12px 24px;
            }}
        }}

        /* Logo styling */
        .brand-logo {{
            display: flex;
            align-items: center;
            justify-content: center;
        }}

        .brand-icon {{
            font-size: 29px;
            margin-right: 8px;
        }}

        .brand-name {{
            font-size: 25px;
            font-weight: 500;
        }}

        .brand-primary {{
            color: #1d57a4;
        }}

        .brand-secondary {{
            color: #ffffff;
        }}

        /* Button style */
        .btn {{
            display: inline-block;
            background-color: #1a73e8;
            color: white;
            font-family: 'Google Sans', Roboto, Arial, sans-serif;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            padding: 10px 24px;
            border-radius: 4px;
            margin-top: 16px;
        }}

        .help-text {{
            margin-top: 24px;
            font-size: 13px;
            color: #5f6368;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""brand-logo"">
                <span class=""brand-icon"">🥛</span>
                <span class=""brand-name"">
                    <span class=""brand-primary"">Milk</span><span class=""brand-secondary"">Store</span>
                </span>
            </div>
        </div>

        <div class=""content"">
            <div class=""greeting"">Xin chào <strong>{recipientName}</strong>,</div>

            <div class=""message"">
                Chúng tôi đã nhận được yêu cầu xác thực tài khoản của bạn. Vui lòng sử dụng mã xác thực sau để hoàn tất quá trình đăng nhập:
            </div>

            <div class=""otp-container"">
                <div class=""otp-code"">{otpCode}</div>
                <div class=""expiry-note"">Mã này sẽ hết hạn sau <strong>5 phút</strong>.</div>
            </div>

            <div class=""warning"">
                <span class=""warning-icon"">⚠️</span> Vui lòng không chia sẻ mã này với bất kỳ ai, kể cả nhân viên MilkStore. Đội ngũ hỗ trợ của chúng tôi sẽ không bao giờ yêu cầu mã xác thực của bạn.
            </div>

            <div class=""help-text"">
                Nếu bạn không yêu cầu mã này, có thể có người đang cố gắng truy cập vào tài khoản của bạn. Vui lòng không chia sẻ mã này và liên hệ với chúng tôi ngay lập tức.
            </div>

            <div class=""divider""></div>

            <div class=""message"">
                Cảm ơn bạn đã sử dụng dịch vụ của MilkStore!
            </div>
        </div>

        <div class=""footer"">
            <p>© 2025 MilkStore. Tất cả các quyền được bảo lưu.</p>
            <p>Đây là email tự động, vui lòng không trả lời email này.</p>
            <p>MilkStore - Địa chỉ: 123 Đường Nguyễn Văn Linh, Quận 7, TP. Hồ Chí Minh</p>
        </div>
    </div>
</body>
</html>
";
        }
    }
}