using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Repositories.UserRepository;
using Services.AuthenServices.InterfaceAuthen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly string AVATAR_DEFAULT = "https://res.cloudinary.com/ddg2gdfee/image/upload/v1750643519/default_avata_dry3fp.png";

        public AuthService(IUserRepository userRepository, IJWTService jwtService, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<ResponseDTO<UserLoginResponseDTO>> LoginHandler(UserLoginRequestDTO loginRequestDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == loginRequestDTOs.Email 
                                                    && u.Password == _passwordHashingService.HashPassword(loginRequestDTOs.Password)
                                                    && u.IsActive == true);
            if (user == null)
            {
                return new ResponseDTO<UserLoginResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email or password is incorrect."
                };
            }

            var result = await _jwtService.AuthenticateUser(user,false);
            return new ResponseDTO<UserLoginResponseDTO>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Login successfully",
                data = result
            };
        }

        public async Task<ResponseDTO<UserLoginResponseDTO>> RefreshToken(string refreshToken)
        {
            var user = _userRepository.GetUser(u => u.RefreshToken == refreshToken).Result;

            if(user == null || user.TokenExpires < DateTime.Now)
            {
                return new ResponseDTO<UserLoginResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Token is expired."
                };
            }

            var result = await _jwtService.AuthenticateUser(user,true);

            return new ResponseDTO<UserLoginResponseDTO>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Refresh token successfully.",
                data = result
            };
        }

        public async Task<ResponseDTO<string>> RegisterHandler(RegisterUserDTO registerUserDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == registerUserDTOs.Email);

            if(user != null)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email already exists, please choose another email."
                };
            }

            User newUser = new User{
                FullName = registerUserDTOs.FullName,
                Avatar = AVATAR_DEFAULT,
                Email = registerUserDTOs.Email,
                Birth = registerUserDTOs.Birth,
                Gender = registerUserDTOs.Gender,
                Password = _passwordHashingService.HashPassword(registerUserDTOs.Password),
                Role = (int)Enums.Role.User,
                IsActive = true,
                IsOnline = false,
            };

            await _userRepository.Add(newUser);

            return new ResponseDTO<string>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "User registered successfully."
            };
        }


        public async Task<ResponseDTO<UserLoginResponseDTO>> SignInGoogle(AuthenticateResult authenticateResult)
        {
       
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;


            var id = claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

            User user = await _userRepository.GetUser(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    FullName = name,
                    Email = email,
                    Avatar = AVATAR_DEFAULT,
                    Role = (int)Enums.Role.User,
                    GoogleId = id,
                    IsActive = true,
                    IsOnline = false,
                };

                await _userRepository.Add(user);
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = id;
                await _userRepository.Update(user);
            }


            var result = await _jwtService.AuthenticateUser(user,false);
            return new ResponseDTO<UserLoginResponseDTO>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Login successfully",
                data = result
            };
        }
    }
}
