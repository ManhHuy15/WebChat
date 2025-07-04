using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.GroupDTOs
{
    public class GroupMemberDTO
    {
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public GroupBaseDTO Group { get; set; }
        public UserBaseDTO User { get; set; }
    }
}
