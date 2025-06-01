using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class MessageStatus
    {
        [Required]
        public int MessageId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Message Message { get; set; }
        public virtual User User { get; set; }

    }
}
