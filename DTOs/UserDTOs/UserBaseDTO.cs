using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UserBaseDTO
    {
        public int? UserId { get; set; }
        public string? Avatar { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
