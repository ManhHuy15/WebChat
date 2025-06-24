using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ClouldinaryServices;
using Services.GroupServices;
using Services.MessageServices;
using System.Security.Claims;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService, ICloudinaryService cloudinaryService)
        {
            _groupService = groupService;
        }

        [HttpGet("my-group")]
        public async Task<IActionResult> GetMyGroup()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            try
            {
                var result = await _groupService.getMyGroups(int.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
