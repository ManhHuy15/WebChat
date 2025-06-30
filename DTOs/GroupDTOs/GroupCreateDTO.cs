using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.GroupDTOs
{
    public class GroupCreateDTO
    {
        public string Name { get; set; }
        public IFormFile? Avatar { get; set; }
        public int AdminId { get; set; }
        public int[] MemberIds { get; set; }
        public bool IsPrivate { get; set; }
    }
}
