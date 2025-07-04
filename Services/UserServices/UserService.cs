using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using DTOs.FriendDTOs;
using DTOs.UserDTOs;
using Microsoft.IdentityModel.Tokens;
using Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserBaseDTO>> AllUser()
        {
            var users =  await _userRepository.AllUser();
            var result = users.Select(x => new UserBaseDTO()
            {
                UserId = x.UserId,
                Avatar = x.Avatar,
                Email = x.Email,
                FullName = x.FullName
            }).ToList();
            return result;
        }

        public async Task<ResponseDTO<UserDetailDTO>> GetUserById(int userId, int myId)
        {
            var user = await _userRepository.GetUser(u => u.UserId == userId);
            if (user == null)
            {
                return new ResponseDTO<UserDetailDTO>()
                {
                    data = new UserDetailDTO(),
                    message = "Not found",
                    success = false,
                    status = HttpStatusCode.NotFound,
                };
            }
            var status = "none";

            var isFriend = user.ReceiverFriendShips.FirstOrDefault(x => x.FriendId == userId && x.UserId == myId && x.Status == (int) Enums.FriendShipStatus.Accepted);
            var myRequest = user.ReceiverFriendShips.FirstOrDefault(x => x.FriendId == userId && x.UserId == myId && x.Status == (int)Enums.FriendShipStatus.Pending);
            var requestMe = user.SenderFriendShips.FirstOrDefault(x => x.UserId == userId  && x.FriendId == myId && x.Status == (int) Enums.FriendShipStatus.Pending);

            if (isFriend != null)
            {
                status = "friend";
            }
            else
            {
                if (myRequest != null)
                {
                    status = "myrequest";
                }
                else if (requestMe != null)
                {
                    status = "requestme";
                }
            }

            var result = new UserDetailDTO()
            {
                UserId = user.UserId,
                Avatar = user.Avatar,
                Email = user.Email,
                FullName = user.FullName,
                Birth = user.Birth,
                Gender = user.Gender,
                Phone = user.Phone,
                FriendStatus = status
            };
            return new ResponseDTO<UserDetailDTO>()
            {
                data = result,
                message = "Success",
                success = true,
                status = HttpStatusCode.OK,
            };

        }
    }
}
