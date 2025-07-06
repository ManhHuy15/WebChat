using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class MyProfileDTO
    {
        public bool? isLinkGoogle { get; set; }
        public bool havePassword { get; set; }
        public string? Avatar { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly? Birth { get; set; }
        public bool? Gender { get; set; }
        public int FriendCount { get; set; }
    }
}
