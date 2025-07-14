using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using DTOs.EmailDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Repositories.UserRepository;
using Services.AuthenServices.InterfaceAuthen;
using Services.EmailServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly string AVATAR_DEFAULT = "https://res.cloudinary.com/ddg2gdfee/image/upload/v1750643519/default_avata_dry3fp.png";
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailSenderService _emailSenderService;

        public AuthService(IUserRepository userRepository, IJWTService jwtService, IPasswordHashingService passwordHashingService, IEmailSenderService emailSenderService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
            _emailSenderService = emailSenderService;
        }

        public async Task<ResponseDTO<UserLoginResponseDTO>> LoginHandler(UserLoginRequestDTO loginRequestDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == loginRequestDTOs.Email 
                                                    && u.Password == _passwordHashingService.HashPassword(loginRequestDTOs.Password));
            if (user == null)
            {
                return new ResponseDTO<UserLoginResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email or password is incorrect."
                };
            }

            if (!user.IsActive)
            {
                var random = new Random();
                var otp = random.Next(1000, 10000).ToString();
                DateTime otpExpires = DateTime.Now.AddMinutes(5);
                user.Otp = otp;
                user.OtpExpires = DateTime.Now.AddMinutes(5);
                await _userRepository.Update(user);
                try
                {
                    await _emailSenderService.SendEmailAsync(new EmailSenderDTO()
                    {
                        Recipient = user.Email,
                        Subject = "Your OTP Code for Registration",
                        Body = $"Your OTP code is: {otp}. It will expire in 5 minutes."
                    });

                    return new ResponseDTO<UserLoginResponseDTO>
                    {
                        status = HttpStatusCode.BadRequest,
                        success = false,
                        message = "User is not active. Please verify your email.",
                        data = new UserLoginResponseDTO
                        {
                            Email = user.Email,
                            IsActive = user.IsActive,
                            OtpExpires = otpExpires,
                        }
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to send OTP email: {ex.Message}");
                    return new ResponseDTO<UserLoginResponseDTO>
                    {
                        status = HttpStatusCode.InternalServerError,
                        success = false,
                        message = "Failed to send OTP email. Please try again later."
                    };
                }
                
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

        public async Task<ResponseDTO<RegisterResponseDTO>> RegisterHandler(RegisterUserDTO registerUserDTOs)
        {
            User user = await _userRepository.GetUser(u => u.Email == registerUserDTOs.Email);

            if(user != null)
            {
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Email already exists, please choose another email."
                };
            }

            var random = new Random();
            var otp = random.Next(1000, 10000).ToString();
            DateTime otpExpires = DateTime.Now.AddMinutes(5);

            User newUser = new User{
                FullName = registerUserDTOs.FullName,
                Avatar = AVATAR_DEFAULT,
                Email = registerUserDTOs.Email,
                Birth = registerUserDTOs.Birth,
                Gender = registerUserDTOs.Gender,
                Password = _passwordHashingService.HashPassword(registerUserDTOs.Password),
                Role = (int)Enums.Role.User,
                IsActive = false,
                IsOnline = false,
                Otp = otp,
                OtpExpires = otpExpires,
            };

            await _userRepository.Add(newUser);

            try
            {
                await _emailSenderService.SendEmailAsync(new EmailSenderDTO()
                {
                    Recipient = newUser.Email,
                    Subject = "Your OTP Code for Registration",
                    Body = $"Your OTP code is: {otp}. It will expire in 5 minutes."
                });

                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.OK,
                    success = true,
                    message = "User registered successfully.",
                    data = new RegisterResponseDTO
                    {
                        Email = newUser.Email,
                        OtpExpires = otpExpires
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send OTP email: {ex.Message}");
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.InternalServerError,
                    success = false,
                    message = "Failed to send OTP email. Please try again later."
                };
            }
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

        public async Task<ResponseDTO<string>> VerifyOTP(VerifyOTPRequestDTO verifyOTP)
        {
            var user = await _userRepository.GetUser(u => u.Email == verifyOTP.Email);
            if (user == null)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "User not found."
                };
            }
            if (user.Otp != verifyOTP.Otp)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Invalid OTP."
                };
            }

            if (user.OtpExpires < DateTime.Now)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "OTP has expired."
                };
            }
            user.IsActive = true;
            await _userRepository.Update(user);
            return new ResponseDTO<string>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "OTP verified successfully."
            };
        }

        public async Task<ResponseDTO<RegisterResponseDTO>> ResendOTP(string email)
        {
            var user = await _userRepository.GetUser(u => u.Email == email);
            if (user == null)
            {
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "User not found."
                };
            }

            //if (user.IsActive)
            //{
            //    return new ResponseDTO<RegisterResponseDTO>
            //    {
            //        status = HttpStatusCode.BadRequest,
            //        success = false,
            //        message = "User is already active."
            //    };
            //}

            if (user.OtpExpires > DateTime.Now)
            {
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Please wait for it to expire before requesting a new one.",
                    data = new RegisterResponseDTO
                    {
                        Email = user.Email,
                        OtpExpires = user.OtpExpires,
                    }
                };

            }

            var random = new Random();
            var otp = random.Next(1000, 10000).ToString();
            DateTime otpExpires = DateTime.Now.AddMinutes(5);

            user.Otp = otp;
            user.OtpExpires = otpExpires;
            await _userRepository.Update(user);
            try
            {
                await _emailSenderService.SendEmailAsync(new EmailSenderDTO()
                {
                    Recipient = user.Email,
                    Subject = "Your OTP Code for Registration",
                    Body = $"Your OTP code is: {otp}. It will expire in 5 minutes."
                });

                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.OK,
                    success = true,
                    message = "OTP resent successfully.",
                    data = new RegisterResponseDTO
                    {
                        Email = user.Email,
                        OtpExpires = otpExpires
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send OTP email: {ex.Message}");
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.InternalServerError,
                    success = false,
                    message = "Failed to send OTP email. Please try again later."
                };
            }
        }

        public  async Task<ResponseDTO<RegisterResponseDTO>> ForgotSendOtp(string email)
        {
            var user = await _userRepository.GetUser(u => u.Email == email);
            if (user == null)
            {
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "User not found."
                };
            }

            if (!user.IsActive)
            {
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "User is not active."
                };
            }

            var random = new Random();
            var otp = random.Next(1000, 10000).ToString();
            DateTime otpExpires = DateTime.Now.AddMinutes(5);

            user.Otp = otp;
            user.OtpExpires = otpExpires;
            await _userRepository.Update(user);
            try
            {
                await _emailSenderService.SendEmailAsync(new EmailSenderDTO()
                {
                    Recipient = user.Email,
                    Subject = "Your OTP Code for Registration",
                    Body = $"Your OTP code is: {otp}. It will expire in 5 minutes."
                });

                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.OK,
                    success = true,
                    message = "OTP resent successfully.",
                    data = new RegisterResponseDTO
                    {
                        Email = user.Email,
                        OtpExpires = otpExpires
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send OTP email: {ex.Message}");
                return new ResponseDTO<RegisterResponseDTO>
                {
                    status = HttpStatusCode.InternalServerError,
                    success = false,
                    message = "Failed to send OTP email. Please try again later."
                };
            }
        }

        public async Task<ResponseDTO<string>> ResetPassword(ResetPasswordDTO resetPassword)
        {
            if (!resetPassword.NewPassword.Equals(resetPassword.ConfirmPassword))
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Password not match"
                };
            }
            var user = await _userRepository.GetUser(u => u.Email == resetPassword.Email);
            if (user == null)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "User not found."
                };
            }

            if (user.Otp != resetPassword.Otp)
            {
                return new ResponseDTO<string>
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Invalid OTP."
                };
            }

            user.Password = _passwordHashingService.HashPassword(resetPassword.NewPassword);
            user.Otp = null;
            user.OtpExpires = null;
            await _userRepository.Update(user);
            return new ResponseDTO<string>
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Password reset successfully."
            };
        }
    }
}
