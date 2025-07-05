using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using DTOs.FriendDTOs;
using DTOs.UserDTOs;
using Microsoft.IdentityModel.Tokens;
using Repositories.UserRepository;
using Services.ClouldinaryServices;
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
        private readonly ICloudinaryService _cloudinaryService;
        public UserService(IUserRepository userRepository, ICloudinaryService cloudinaryService )
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
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

        public async Task<ResponseDTO<MyProfileDTO>> GetMyProfile(int id)
        {
            var user = await _userRepository.GetUser(u => u.UserId == id);
            if (user == null)
            {
                return new ResponseDTO<MyProfileDTO>()
                {
                    data = new MyProfileDTO(),
                    message = "Not found",
                    success = false,
                    status = HttpStatusCode.BadRequest,
                };
            }

            var result = new MyProfileDTO()
            {
                Avatar = user.Avatar,
                Birth   = user.Birth,
                Email = user.Email,
                FullName = user.FullName,
                Gender = user.Gender,
                Phone = user.Phone,
                isLinkGoogle = !string.IsNullOrEmpty(user.GoogleId),
                FriendCount = user.ReceiverFriendShips.Count(x => x.Status == (int)Enums.FriendShipStatus.Accepted),
            };
            return new ResponseDTO<MyProfileDTO>()
            {
                data = result,
                message = "Success",
                success = true,
                status = HttpStatusCode.OK,
            };

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

        public async Task<ResponseDTO<bool>> UpdateAvatar(UpdateAvatarDTO avatar, int myId)
        {
            var userdb = await _userRepository.GetUser(u => u.UserId == myId);
            if (userdb == null)
            {
                return new ResponseDTO<bool>()
                {
                    data = false,
                    message = "User not found",
                    success = false,
                    status = HttpStatusCode.NotFound,
                };
            }

            var uploadResult = await _cloudinaryService.UpLoadFileAsync(avatar.Avatar);

            if (uploadResult == null)
            {
                return new ResponseDTO<bool>()
                {
                    data = false,
                    message = "Upload avatar failed",
                    success = false,
                    status = HttpStatusCode.BadRequest,
                };
            }
            userdb.Avatar = uploadResult.Url.ToString();
            var res = await _userRepository.Update(userdb);
            return new ResponseDTO<bool>()
            {
                data = res,
                message = res ? "Update avatar successful" : "Update avatar failed",
                success = res,
                status = HttpStatusCode.OK,
            };
        }

        public async Task<ResponseDTO<bool>> UpdateName(string name, int myId)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 3)
            {
                return new ResponseDTO<bool>()
                {
                    data = false,
                    message = "Name must be at least 3 characters long",
                    success = false,
                    status = HttpStatusCode.BadRequest,
                };
            }

            var userdb = await _userRepository.GetUser(u => u.UserId == myId);
            if (userdb == null)
            {
                return new ResponseDTO<bool>()
                {
                    data = false,
                    message = "User not found",
                    success = false,
                    status = HttpStatusCode.NotFound,
                };
            }

            userdb.FullName = name;
            var result = await _userRepository.Update(userdb);
            return new ResponseDTO<bool>()
            {
                data = result,
                message = result ? "Update name successful" : "Update name failed",
                success = result,
                status = HttpStatusCode.OK,
            };
        }

        public async Task<ResponseDTO<bool>> UpdateProfile(UpdateInfoUserDTO user, int myId)
        {
            var userdb = await _userRepository.GetUser(u => u.UserId == myId);
            if (userdb == null)
            {
                return new ResponseDTO<bool>()
                {
                    data = false,
                    message = "User not found",
                    success = false,
                    status = HttpStatusCode.NotFound,
                };
            }
            if (user.Birth != null)
            {
                userdb.Birth = user.Birth;
            }
            if (!string.IsNullOrEmpty(user.Phone))
            {
                userdb.Phone = user.Phone;
            }
            if (user.Gender != null)
            {
                userdb.Gender = user.Gender;
            }

            var res = await _userRepository.Update(userdb);

            return new ResponseDTO<bool>()
            {
                data = res,
                message = res ? "Update successful" : "Update failed",
                success = res,
                status = HttpStatusCode.OK,
            };
        }
    }
}
