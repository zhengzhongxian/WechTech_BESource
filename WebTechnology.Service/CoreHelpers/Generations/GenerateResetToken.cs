﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.CoreHelpers.Generations
{
    public class GenerateResetToken
    {
        public static string Generate()
        {
            // Tạo token ngẫu nhiên có độ dài 64 ký tự
            var randomBytes = new byte[32]; // 32 bytes = 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            
            // Chuyển đổi thành chuỗi base64 và loại bỏ các ký tự đặc biệt
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
    }
}
