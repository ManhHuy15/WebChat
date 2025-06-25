using DTOs.GroupDTOs;
using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.MessageDTOs
{
    public class MessageGroupDTO : MessageDTO
    {
        public UserBaseDTO Sender { get; set; } = new UserBaseDTO();
        public GroupBaseDTO Group { get; set; } = new GroupBaseDTO();
    }
}
