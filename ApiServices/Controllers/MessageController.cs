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

        [HttpPost("send-message/{receiverId}")]
        public async Task<IActionResult> SendMessage([FromRoute] int receiverId,[FromForm] SendMessageDTO message)
        {
            try
            {
                foreach (var file in message.files)
                {
                    var result = await _cloudinaryService.UpLoadFileAsync(file);
                    Console.WriteLine(result);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
