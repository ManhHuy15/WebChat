using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.FriendDTOs
{
    public class FriendAddUpdateDTO
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public int Status { get; set; }
    }
}
