using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.MessageServices;
using System.Security.Claims;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("list-chat")]
        public async Task<IActionResult> GetListChat()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _messageService.GetListChat(int.Parse(userId));
                return Ok(result);
            }catch( Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-messages/{receiverId}")]
        public async Task<IActionResult> GetMessages([FromRoute] int receiverId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _messageService.GetAllMessagesUser(int.Parse(userId), receiverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
