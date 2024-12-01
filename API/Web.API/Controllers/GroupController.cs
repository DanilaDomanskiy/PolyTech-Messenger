using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.Group;
using Web.Application.Services;
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
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var groupId = await _groupService.CreateAsync(currentUserId, createGroupDto);
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

        [HttpDelete("{groupId}")]
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

        [HttpGet("{groupId}")]
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

        [HttpGet("users/{groupId}")]
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

        [HttpPatch("image/{groupId}")]
        public async Task<IActionResult> UpdateGroupImage(Guid groupId, IFormFile? file)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (!await _groupService.IsUserGroupCreatorAsync(currentUserId, groupId))
                {
                    return Forbid();
                }

                string profileImagesFolder = Path.Combine(_folderPath, "profile-images");

                Directory.CreateDirectory(profileImagesFolder);

                string? oldFile;

                if (file == null || file.Length == 0)
                {
                    oldFile = Directory.GetFiles(Path.Combine(_folderPath, "group-images"), $"{currentUserId}.*")
                        .FirstOrDefault();
                    if (oldFile != null) System.IO.File.Delete(oldFile);
                    await _groupService.UpdateGroupImageAsync(groupId, null);
                    return Created();
                }
                else
                {
                    if (file.Length > 5 * 1024 * 1024) return StatusCode(409);

                    try
                    {
                        using var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
                    }
                    catch (SixLabors.ImageSharp.UnknownImageFormatException)
                    {
                        return StatusCode(409);
                    }
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var path = $"group-images/{currentUserId}{extension}";
                var filePath = Path.Combine(_folderPath, path);

                oldFile = Directory.GetFiles(Path.Combine(_folderPath, "group-images"), $"{currentUserId}.*")
                    .FirstOrDefault();
                if (oldFile != null) System.IO.File.Delete(oldFile);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                await _groupService.UpdateGroupImageAsync(groupId, path);

                return Created();
            }
            return Unauthorized();
        }

        [HttpPatch("name")]
        public async Task<IActionResult> UpdateGroupName([FromBody] GroupNameDto groupNameDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, groupNameDto.GroupId))
                {
                    await _groupService.UpdateGroupNameAsync(groupNameDto);
                    return NoContent();
                }
                return Forbid();
            }
            return Unauthorized();
        }
    }
}