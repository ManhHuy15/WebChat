using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DTOs.EmailDTOs;

namespace Services.EmailServices
{
    public class EmailSenderService : IEmailSenderService
    {

        private readonly GmailSettings _gmailSettings;

        public EmailSenderService(GmailSettings gmailSetting)
        {
            _gmailSettings = gmailSetting;
        }

        public async Task SendEmailAsync(EmailSenderDTO senderDTO)
        {
            MailMessage mailMessage = new MailMessage(_gmailSettings.Email, senderDTO.Recipient, senderDTO.Subject, senderDTO.Body)
            {
                IsBodyHtml = true
            };

            using (var client = new SmtpClient(_gmailSettings.Host, _gmailSettings.Port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_gmailSettings.Email, _gmailSettings.Password);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
