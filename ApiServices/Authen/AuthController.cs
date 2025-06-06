using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthenServices.InterfaceAuthen;
using Services.UserServices;

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
        public async Task<IActionResult> Login( [FromBody] UserLoginRequestDTOs userLogin)
        {
            if(ModelState.IsValid == false) return BadRequest(ModelState);
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
        public async Task<IActionResult> Register([FromBody] RegisterUserDTOs userRegister)
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

    }
}
