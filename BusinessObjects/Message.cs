using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        [Required]
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public int Type { get; set; }

        public virtual User Sender { get; set; } = new User();
        public virtual User? Receiver { get; set; }
        public virtual Group? Group { get; set; }
        public virtual ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
    }
}
