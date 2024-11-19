using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.Group;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] string groupName)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var group = new CreateGroupDto
                {
                    CreatorId = currentUserId,
                    Name = groupName
                };
                var groupId = await _groupService.CreateAsync(group);
                return Ok(groupId);
            }
            return Unauthorized();
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddUserToGroup([FromBody] AddUserToGroupDto model)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, model.GropId))
                {
                    await _groupService.AddUserAsync(model);
                    return NoContent();
                }
                return Forbid();
            }
            return Unauthorized();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGroups()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var chats = await _groupService.GetGroupsAsync(currentUserId);
                return Ok(chats);
            }
            return Unauthorized();
        }

        [HttpDelete("/{groupId}")]
        public async Task<IActionResult> DeleteAsync(Guid groupId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, groupId))
                {
                    await _groupService.DeleteAsync(groupId);
                    return NoContent();
                }
                return Forbid();
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> ReadGroup(Guid groupId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);
                if (!isUserExist)
                {
                    return Forbid();
                }
                var chat = await _groupService.GetGroupAsync(groupId, currentUserId);
                return Ok(chat);
            }
            return Unauthorized();
        }
    }
}