using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class ChatItemDTO
    {
        public int? Id { get; set; }
        public int Type { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? ContentPreview { get; set; }
        public DateTime? Time { get; set; }
        public bool IsRead { get; set; }
    }
}