using DTOs.FriendDTOs;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.UserDTOs;

namespace Services.FriendServices
{
    public interface IFriendService
    {
        Task<List<UserBaseDTO>> getMyFriends(int userId);

        Task<ResponseDTO<List<FriendDTO>>> getMyRequests(int userId);

        Task<ResponseDTO<List<FriendDTO>>> getRequestsToMe(int userId);
    }
}
