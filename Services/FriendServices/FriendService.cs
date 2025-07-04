using BusinessObjects;
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

        public async Task<ResponseDTO<bool>> AcceptFriend(FriendAddUpdateDTO newFriend)
        {
            var updateFriendship = new FriendShip()
            {
                UserId = newFriend.UserId,
                FriendId = newFriend.FriendId,
                Status = (int)Enums.FriendShipStatus.Accepted,
            };

            var result = await _friendRepository.UpdateFriend(updateFriendship);

            if (result)
            {
                var newFriendship = new FriendShip()
                {
                    UserId = newFriend.FriendId,
                    FriendId = newFriend.UserId,
                    Status = (int)Enums.FriendShipStatus.Accepted,
                };
                
                var resultAdd = await _friendRepository.AddFriend(newFriendship);

                if (resultAdd)
                {
                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        data = true,
                        message = "Accept friend successfully",
                        status = HttpStatusCode.OK,
                    };
                }
            }

            return new ResponseDTO<bool>()
            {
                success = false,
                data = false,
                message = "Accept friend failed",
                status = HttpStatusCode.BadRequest,
            };
        }

        public async Task<ResponseDTO<bool>> AddFriend(FriendAddUpdateDTO newFriend)
        {
            var newFriendship = new FriendShip()
            {
                UserId = newFriend.UserId,
                FriendId = newFriend.FriendId,
                Status = (int) Enums.FriendShipStatus.Pending,
            };

            var result = await _friendRepository.AddFriend(newFriendship);

            if (!result)
            {
                return new ResponseDTO<bool>()
                {
                    success = false,
                    data = false,
                    message = "Add friend failed",
                    status = HttpStatusCode.BadRequest,
                };
            }

            return new ResponseDTO<bool>()
            {
                success = true,
                data = true,
                message = "Add friend successfully",
                status = HttpStatusCode.OK,
            };
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

        public async Task<ResponseDTO<bool>> RemoveFriend(RemoveFriendDTO friendDTO)
        {
            var result = await _friendRepository.RemoveFriend(friendDTO.UserId, friendDTO.FriendId);

            if (!result)
            {
                return new ResponseDTO<bool>()
                {
                    success = false,
                    data = false,
                    message = "Remove friend failed",
                    status = HttpStatusCode.BadRequest,
                };
            }

            return new ResponseDTO<bool>()
            {
                success = true,
                data = true,
                message = "Remove friend successfully",
                status = HttpStatusCode.OK,
            };
        }
    }
}
