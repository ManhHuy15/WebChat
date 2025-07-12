using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserServices
{
    public interface IUserService
    {
        Task<List<UserBaseDTO>> AllUser();
        Task<ResponseDTO<UserDetailDTO>> GetUserById(int userId,int myId);
        Task<ResponseDTO<MyProfileDTO>> GetMyProfile(int id);
        Task<ResponseDTO<bool>> UpdateProfile(UpdateInfoUserDTO user, int myId);
        Task<ResponseDTO<bool>> UpdateAvatar(UpdateAvatarDTO avatar, int myId);
        Task<ResponseDTO<bool>> UpdateName(string name, int myId);
        Task<ResponseDTO<bool>> UpdatePassword(UpdatePasswordDTO password, int myId);
        Task<ResponseDTO<bool>> InitPassword(InitPasswordDTO password, int myId);
    }
}
