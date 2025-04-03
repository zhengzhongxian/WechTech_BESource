using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        /// <summary>
        /// secret key cho Access Token (64 ký tự) ? => do algorithm là HS512
        /// </summary>
        public string AccessTokenKey { get; set; }

        /// <summary>
        /// secret key cho Refresh Token (64 ký tự)
        /// </summary>
        public string RefreshTokenKey { get; set; }

        /// <summary>
        /// thời gian sống của Access Token (phút)
        /// </summary>
        public int AccessTokenExpiryMinutes { get; set; }

        /// <summary>
        /// thời gian sống của Refresh Token (ngày)
        /// </summary>
        public int RefreshTokenExpiryDays { get; set; }

        /// <summary>
        /// dung sai thời gian khi validate token (phút)
        /// </summary>
        public int ClockSkewMinutes { get; set; }

        /// <summary>
        /// thuật toán ký token
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// chuyển đổi ClockSkewMinutes sang TimeSpan
        /// </summary>
        public TimeSpan ClockSkew => TimeSpan.FromMinutes(ClockSkewMinutes);

    }
}
