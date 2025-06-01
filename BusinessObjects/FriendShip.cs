using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class FriendShip
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public int Status { get; set; }

        public virtual User User { get; set; }
        public virtual User Friend { get; set; }
    }
}
