using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class MessageUserDTO : MessageDTO
    {
        public UserBaseDTO Sender { get; set; } = new UserBaseDTO();
        public UserBaseDTO Receiver { get; set; } = new UserBaseDTO();
    }
}
