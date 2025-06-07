using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices.InterfaceAuthen
{
    public interface IAuthService 
    {
        Task<ResponseDTO<UserLoginResponseDTOs>> LoginHandler(UserLoginRequestDTOs loginRequestDTOs);

        Task<ResponseDTO<string>> RegisterHandler(RegisterUserDTOs registerUserDTOs);

        Task<ResponseDTO<UserLoginResponseDTOs>> SignInGoogle(AuthenticateResult authenticateResult);
    }
}
