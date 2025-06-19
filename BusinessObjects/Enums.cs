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

        public enum ChatItemType
        {
            User,
            Group
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

        public static int GetResourceTypeId(string resourceName)
        {
            switch (resourceName)
            {
                case "image":
                    return (int)MessageType.Image;
                case "video":
                    return (int)MessageType.Video;
                case "audio":
                    return (int)MessageType.Audio;
                case "raw":
                    return (int)MessageType.File;
                default:
                    return (int)MessageType.Text;
            }
        }
    }
}
