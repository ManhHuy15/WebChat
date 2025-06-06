using BusinessObjects;
using DTOs.AuthenDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices.InterfaceAuthen
{
    public interface IJWTService
    {
        Task<UserLoginResponseDTOs> AuthenticateUser(User loginUser);
        Task<UserLoginResponseDTOs> AuthenticateUserWithRefreshToken(string RequsetRefreshToken);
        string GenerateRefreshToken();
    }
}
