using BusinessObjects;
using CloudinaryDotNet.Actions;
using DTOs.MessageDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.ClouldinaryServices;
using Services.MessageServices;
using System.Security.Claims;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ICloudinaryService _cloudinaryService;

        public MessageController(IMessageService messageService, ICloudinaryService cloudinaryService)
        {
            _messageService = messageService;
            _cloudinaryService = cloudinaryService;
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

        [HttpGet("get-messages/{type}/{receiverId}")]
        public async Task<IActionResult> GetMessages([FromRoute] string type, [FromRoute] int receiverId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                if (type == "group")
                {
                    var result = await _messageService.GetAllMessagesInGroup( receiverId);
                    return Ok(result);
                }
                else if (type == "user")
                {
                    var result = await _messageService.GetAllMessagesUser(int.Parse(userId), receiverId);
                    return Ok(result);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("send-message/{type}/{receiverId}")]
        public async Task<IActionResult> SendMessage([FromRoute] int type, [FromRoute] int receiverId,[FromForm] SendMessageDTO message)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))  return Unauthorized();
            if (message.Files == null && string.IsNullOrEmpty(message.Content)) return BadRequest("No content");
            try
            {
                 var result = await _messageService.SendMessage(int.Parse(userId), type, message);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-user-message-file/{friendId}")]
        public async Task<IActionResult> GetUserMessageFiles(int friendId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var result = await _messageService.GetUserMessageFile(int.Parse(userId), friendId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
