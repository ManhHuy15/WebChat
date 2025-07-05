using BusinessObjects;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services.UserServices;
using System.Security.Claims;

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

        [HttpGet("get-all")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _userService.AllUser();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _userService.GetUserById(id, int.Parse(myId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(myId))
                {
                    return Unauthorized();
                }
                var result = await _userService.GetMyProfile(int.Parse(myId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateInfoUserDTO userUpdateDTO)
        {
            try
            {
                var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(myId))
                {
                    return Unauthorized();
                }
                var result = await _userService.UpdateProfile(userUpdateDTO, int.Parse(myId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("update-avatar")]
        public async Task<IActionResult> UpdateAvatar(UpdateAvatarDTO user)
        {
            try
            {
                var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(myId))
                {
                    return Unauthorized();
                }
                var result = await _userService.UpdateAvatar(user, int.Parse(myId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("update-name")]
        public async Task<IActionResult> UpdateName([FromBody] string fullName)
        {
            try
            {
                var myId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(myId))
                {
                    return Unauthorized();
                }
                var result = await _userService.UpdateName(fullName, int.Parse(myId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
