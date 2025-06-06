using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using Repositories.UserRepository;
using Services.AuthenServices.InterfaceAuthen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        

        public AuthService(IUserRepository userRepository, IJWTService jwtService, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<ResponseDTO> LoginHandler(UserLoginRequestDTOs loginRequestDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == loginRequestDTOs.Email 
                                                    && u.Password == _passwordHashingService.HashPassword(loginRequestDTOs.Password)
                                                    && u.IsActive == true);
            if (user == null)
            {
                return new ResponseDTO
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email or password is incorrect."
                };
            }

            var result = await _jwtService.AuthenticateUser(user);
            return new ResponseDTO
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Login successfully",
                data = result
            };
        }

        public async Task<ResponseDTO> RegisterHandler(RegisterUserDTOs registerUserDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == registerUserDTOs.Email);

            if(user != null)
            {
                return new ResponseDTO
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email already exists, please choose another email."
                };
            }

            User newUser = new User{
                FullName = registerUserDTOs.FullName,
                Email = registerUserDTOs.Email,
                Birth = registerUserDTOs.Birth,
                Gender = registerUserDTOs.Gender,
                Password = _passwordHashingService.HashPassword(registerUserDTOs.Password),
                Role = (int)Enums.Role.User,
                IsActive = true,
                IsOnline = false,
            };

            await _userRepository.Add(newUser);

            return new ResponseDTO
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "User registered successfully."
            };
        }
    }
}
