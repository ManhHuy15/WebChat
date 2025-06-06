using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.UserServices;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
