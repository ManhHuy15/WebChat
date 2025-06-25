using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class MessageStatusDTO
    {
        public bool IsRead { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public UserBaseDTO User { get; set; }
    }
}
