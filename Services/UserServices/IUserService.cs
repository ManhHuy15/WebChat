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

        Task<ResponseDTO<UserDetailDTO>> GetUserById(int id);

    }
}
