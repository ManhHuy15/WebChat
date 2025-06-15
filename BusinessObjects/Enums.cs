using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Enums
    {
        public enum FriendShipStatus
        {
            Pending,
            Accepted
        }
        public enum MessageType
        {
            Text,
            Image,
            Video,
            Audio,
            File
        }
        public enum Role
        {
            User,
            Admin,
        }

        public static string GetRoleName(int role)
        {
            switch (role)
            {
                case (int) Role.User:
                    return "User";
                case (int) Role.Admin:
                    return "Admin";
                default:
                    return "User";
            }
        }
    }
}
