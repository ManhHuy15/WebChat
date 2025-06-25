using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class MessageDTO
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public int Type { get; set; }
        public virtual ICollection<MessageStatusDTO> MessageStatuses { get; set; } = new List<MessageStatusDTO>();
    }
}
