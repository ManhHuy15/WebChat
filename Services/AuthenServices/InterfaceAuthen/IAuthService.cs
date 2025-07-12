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
        Task<ResponseDTO<UserLoginResponseDTO>> LoginHandler(UserLoginRequestDTO loginRequestDTOs);

        Task<ResponseDTO<RegisterResponseDTO>> RegisterHandler(RegisterUserDTO registerUserDTOs);

        Task<ResponseDTO<UserLoginResponseDTO>> SignInGoogle(AuthenticateResult authenticateResult);

        Task<ResponseDTO<UserLoginResponseDTO>> RefreshToken(string refreshToken);
        Task<ResponseDTO<string>> VerifyOTP(VerifyOTPRequestDTO verifyOTP);

        Task<ResponseDTO<RegisterResponseDTO>> ResendOTP(string email);
    }
}
