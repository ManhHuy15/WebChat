using DTOs.FriendDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UserDetailDTO : UserBaseDTO
    {
        public string? Phone { get; set; }
        public string Email { get; set; }
        public DateOnly? Birth { get; set; }
        public bool? Gender { get; set; }
        public string FriendStatus { get; set; }
        public  List<FriendDTO> SenderFriendShips { get; set; } = new List<FriendDTO>();
        public List<FriendDTO> ReceiverFriendShips { get; set; } = new List<FriendDTO>();
    }
}
