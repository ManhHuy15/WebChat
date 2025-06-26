using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.FriendDTOs
{
    public class FriendDTO
    {
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public int Status { get; set; }

        public virtual UserBaseDTO User { get; set; }
        public virtual UserBaseDTO  Friend { get; set; }
    }
}
