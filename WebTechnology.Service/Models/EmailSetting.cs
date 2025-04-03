using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    public class EmailSetting
    {
        public string FromEmailAddress { get; set; } = null!;
        public string FromDisplayName { get; set; } = null!;
        public Smtp Smtp { get; set; } = null!;
    }
    public class Smtp
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; }
        public bool UseCredential { get; set; }
    }
}
