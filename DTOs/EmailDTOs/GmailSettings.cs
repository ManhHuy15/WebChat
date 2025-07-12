using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.EmailDTOs
{
    public class GmailSettings
    {
        public const string GmailSettingsKey = "GmailSettings";

        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
