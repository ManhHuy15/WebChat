using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        public string? Avatar { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AdminId { get; set; }
        public bool IsPrivate { get; set; }

        public virtual User Admin { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
