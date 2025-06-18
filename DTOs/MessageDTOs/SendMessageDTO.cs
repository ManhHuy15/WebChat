using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class SendMessageDTO
    {
        public int ReceiverId { get; set; }
        public string? Content { get; set; }
        public List<IFormFile>? files { get; set; }

    }
}

