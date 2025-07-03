using DTOs.GroupDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services.ClouldinaryServices;
using Services.GroupServices;
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
        [EnableQuery]
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

        [HttpGet("detail/{groupId}")]
        [EnableQuery]
        public async Task<IActionResult> GetDetail(int groupId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                var result = await _groupService.getDetails(int.Parse(userId),groupId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("leave/{groupId}")]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                var result = await _groupService.RemoveMemberFromGroup(int.Parse(userId), groupId);
                return Ok(result);
            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromForm] GroupCreateDTO group)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                group.AdminId = int.Parse(userId);
                var result = await _groupService.CreateGroup(group);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGroup([FromForm] GroupUpdateDTO group, int id)
        {
            try
            {
                var result = await _groupService.UpdateGroup(id, group);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-members/{groupId}")]
        [EnableQuery]
        public async Task<IActionResult> GetMember(int groupId)
        {
            try
            {
                var result = await _groupService.GetMembers(groupId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("add-members/{groupId}")]
        public async Task<IActionResult> AddMember(int groupId, [FromBody] AddMemberDTO members)
        {
            try
            {
                var result = await _groupService.AddMembersToGroup(groupId, members);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
