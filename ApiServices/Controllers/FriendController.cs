using DTOs.FriendDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services.FriendServices;
using System.Security.Claims;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpGet("get-my-friends")]
        [EnableQuery]
        public async Task<IActionResult> getMyFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _friendService.getMyFriends(int.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-my-requests")]
        public async Task<IActionResult> getMyRequest()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _friendService.getMyRequests(int.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-requests-to-me")]
        public async Task<IActionResult> getRequestToMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _friendService.getRequestsToMe(int.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFriend([FromBody] int friendId)
        {
            try
            { 
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                FriendAddUpdateDTO friend = new FriendAddUpdateDTO();
                friend.UserId = int.Parse(userId);
                friend.FriendId = friendId;

                var result = await _friendService.AddFriend(friend);
                return Ok(result);
            }
            catch (Exception ex){
                return BadRequest(ex);
            }
        }

        [HttpPut("accept")]
        public async Task<IActionResult> AcceptFriend([FromBody] int friendId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                FriendAddUpdateDTO friend = new FriendAddUpdateDTO();
                friend.FriendId = int.Parse(userId);
                friend.UserId = friendId;

                var result = await _friendService.AcceptFriend(friend);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFriend([FromBody] int friendId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                RemoveFriendDTO friend = new RemoveFriendDTO();
                friend.UserId = int.Parse(userId);
                friend.FriendId = friendId;

                var result = await _friendService.RemoveFriend(friend);
                return Ok(result);

            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
