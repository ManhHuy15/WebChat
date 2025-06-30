using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.UserDTOs;

namespace DTOs.GroupDTOs
{
    public class GroupInfoDTO
    {
        public int GroupId { get; set; }
        public string? Avatar { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsPrivate { get; set; }
        public  UserBaseDTO Admin { get; set; }
        public  int memberCount { get; set; }
    }
}
