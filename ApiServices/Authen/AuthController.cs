using DTOs;
using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Services.AuthenServices.InterfaceAuthen;
using System.Net;
using System.Security.Claims;

namespace ApiServices.Authen
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userLogin)
        {
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            try
            {
                var result = await _authService.LoginHandler(userLogin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userRegister)
        {
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            try
            {
                var response = await _authService.RegisterHandler(userRegister);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var respone = await _authService.RefreshToken(refreshToken);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("google")]
        public async Task<IActionResult> SignInGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var response = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                if (!response.Succeeded)
                {
                    return Unauthorized(new ResponseDTO<string>
                    {
                        status = HttpStatusCode.Unauthorized,
                        success = false,
                        message = "Unauthorized"
                    });
                }

                var result = await _authService.SignInGoogle(response);

                var html = $@"
                        <script>
                            window.opener.postMessage({{ 
                                UserId: '{result.data.UserId}',
                                Status: '{result.status.GetHashCode()}',
                                Email: '{result.data.Email}',
                                AccessToken: '{result.data.AccessToken}',
                                RefreshToken: '{result.data.RefreshToken}',
                                TokenExpires: '{result.data.TokenExpires}'}},
                                'http://localhost:5108');
                            window.close();
                        </script>
                    ";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyOTPRequestDTO verifyOTP)
        {
            try
            {
                var respone = await _authService.VerifyOTP(verifyOTP);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }
                var user = await _authService.ResendOTP(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
