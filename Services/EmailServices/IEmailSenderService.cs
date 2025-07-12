using DTOs.EmailDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.EmailServices
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailSenderDTO senderDTO);
    }
}
