using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    /// <summary>
    /// Cấu hình cho Payos Payment Gateway
    /// </summary>
    public class PayosSettings
    {
        /// <summary>
        /// Client ID được cung cấp bởi Payos
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// API Key được cung cấp bởi Payos
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Checksum Key được cung cấp bởi Payos
        /// </summary>
        public string ChecksumKey { get; set; }

        /// <summary>
        /// URL cơ sở của API Payos
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
