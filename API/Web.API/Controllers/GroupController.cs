using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.Group;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API.Controllerss
{
    [Authorize]
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly string _folderPath;

        public GroupController(IGroupService groupService, IConfiguration configuration)
        {
            _groupService = groupService;
            _folderPath = configuration["FileStorageSettings:UploadFolderPath"];
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
        public async Task<IActionResult> AddUserToGroup([FromBody] GroupUserDto model)
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

        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUserFromGroup([FromBody] GroupUserDto model)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, model.GropId))
                {
                    await _groupService.DeleteUserAsync(model);
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

        [HttpGet("users")]
        public async Task<IActionResult> ReadGroupUsers(Guid groupId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);
                if (!isUserExist)
                {
                    return Forbid();
                }
                var users = await _groupService.GetGroupUsersAsync(groupId);
                return Ok(users);
            }
            return Unauthorized();
        }

        [HttpGet("image")]
        public IActionResult GetGroupImage(string imagePath)
        {
            var fileFullPath = Path.Combine(_folderPath, imagePath);
            if (!System.IO.File.Exists(fileFullPath))
            {
                return NotFound();
            }
            return PhysicalFile(fileFullPath, "application/octet-stream");
        }

        [HttpPatch("name")]
        public async Task<IActionResult> UpdateGroupName([FromBody] GroupNameDto groupNameDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, groupNameDto.GroupId))
                {
                    await _groupService.ChangeGroupNameAsync(groupNameDto);
                    return NoContent();
                }
                return Forbid();
            }
            return Unauthorized();
        }
    }
}