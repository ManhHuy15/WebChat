using DTOs;
using DTOs.FriendDTOs;
using DTOs.UserDTOs;
using Repositories.FriendRepository;
using Repositories.GroupRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.FriendServices
{
    public class FriendService : IFriendService
    {

        private readonly IFriendRepository _friendRepository;

        public FriendService(IFriendRepository friendRepository)
        {
            _friendRepository = friendRepository;
        }
        public async Task<List<UserBaseDTO>> getMyFriends(int userId)
        {
            var friends = await _friendRepository.getMyFriendships(userId);

            if (friends == null)
            {
                return new List<UserBaseDTO>();
            };

            var result = friends.Select(x => new UserBaseDTO()
            {
                FullName = x.Friend.FullName,
                UserId = x.FriendId,
                Avatar = x.Friend.Avatar,
                Email = x.Friend.Email
            }).ToList();

            return result;
        }

        public async Task<ResponseDTO<List<FriendDTO>>> getMyRequests(int userId)
        {
            var friends = await _friendRepository.getMyRequest(userId);

            if (friends == null)
            {
                return new ResponseDTO<List<FriendDTO>>()
                {
                    success = false,
                    data = new List<FriendDTO>(),
                    message = "You have no friends",
                    status = HttpStatusCode.NotFound,
                };
            }
            ;

            var result = friends.Select(x => new FriendDTO()
            {

                User = new UserBaseDTO()
                {
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    Avatar = x.User.Avatar
                },
                Friend = new UserBaseDTO()
                {
                    UserId = x.FriendId,
                    FullName = x.Friend.FullName,
                    Email = x.Friend.Email,
                    Avatar = x.Friend.Avatar
                },
                Status = x.Status,
                UpdateAt =  x.UpdateAt,
            }).ToList();

            return new ResponseDTO<List<FriendDTO>>()
            {
                success = true,
                data = result,
                message = "get friends successfully",
                status = HttpStatusCode.OK,
            };
        }

        public async Task<ResponseDTO<List<FriendDTO>>> getRequestsToMe(int userId)
        {
            var friends = await _friendRepository.getRequestToMe(userId);

            if (friends == null)
            {
                return new ResponseDTO<List<FriendDTO>>()
                {
                    success = false,
                    data = new List<FriendDTO>(),
                    message = "You have no friends",
                    status = HttpStatusCode.NotFound,
                };
            }
            ;

            var result = friends.Select(x => new FriendDTO()
            {

                User = new UserBaseDTO()
                {
                    UserId = x.UserId,
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    Avatar = x.User.Avatar
                },
                Friend = new UserBaseDTO()
                {
                    FullName = x.Friend.FullName,
                    Email = x.Friend.Email,
                    Avatar = x.Friend.Avatar
                },
                Status = x.Status,
                UpdateAt = x.UpdateAt,
            }).ToList();

            return new ResponseDTO<List<FriendDTO>>()
            {
                success = true,
                data = result,
                message = "get friends successfully",
                status = HttpStatusCode.OK,
            };
        }
    }
}
