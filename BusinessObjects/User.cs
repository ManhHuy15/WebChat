using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(25)]
        public string? GoogleId { get; set; }

        public string? Avatar { get; set; }

        [Required,MaxLength(150)]
        public string FullName { get; set; }
        [MaxLength(10)]
        public string? Phone { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(150)]
        public string? UserName { get; set; }

        public DateOnly? Birth { get; set; }

        public bool? Gender { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        public DateTime? LastActive { get; set; } 

        public bool IsOnline { get; set; }

        public int Role { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? TokenExpires { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<FriendShip> SenderFriendShips { get; set; } = new List<FriendShip>();
        public virtual ICollection<FriendShip> ReceiverFriendShips { get; set; } = new List<FriendShip>();
        public virtual ICollection<Message> SenderMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceiverMessages { get; set; } = new List<Message>();
        public virtual ICollection<GroupMember> GroupMembersShips { get; set; } = new List<GroupMember>();
        public virtual ICollection<Group> AdminGroups { get; set; } = new List<Group>();
        public virtual ICollection<MessageStatus> MessageStatus { get; set; } = new List<MessageStatus>();
    }
}
