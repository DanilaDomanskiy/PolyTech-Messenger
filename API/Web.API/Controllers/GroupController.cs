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
        private readonly ILogger<GroupController> _logger;
        private readonly string _folderPath;

        public GroupController(IGroupService groupService, IConfiguration configuration, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _folderPath = configuration["FileStorageSettings:UploadFolderPath"];
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
        {
            _logger.LogInformation("Attempting to create a group...");
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var groupId = await _groupService.CreateAsync(currentUserId, createGroupDto);
                _logger.LogInformation("Group created successfully with ID: {GroupId}", groupId);
                return Ok(groupId);
            }
            _logger.LogWarning("Unauthorized attempt to create a group.");
            return Unauthorized();
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddUserToGroup([FromBody] GroupUserDto model)
        {
            _logger.LogInformation("Attempting to add user to group with ID: {GroupId}", model.GropId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, model.GropId))
                {
                    await _groupService.AddUserAsync(model);
                    _logger.LogInformation("User added to group successfully.");
                    return NoContent();
                }
                _logger.LogWarning("Forbidden: User is not the creator of the group.");
                return Forbid();
            }
            _logger.LogWarning("Unauthorized attempt to add user to group.");
            return Unauthorized();
        }

        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUserFromGroup([FromBody] GroupUserDto model)
        {
            _logger.LogInformation("Attempting to delete user from group with ID: {GroupId}", model.GropId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, model.GropId))
                {
                    await _groupService.DeleteUserAsync(model);
                    _logger.LogInformation("User deleted from group successfully.");
                    return NoContent();
                }
                _logger.LogWarning("Forbidden: User is not the creator of the group.");
                return Forbid();
            }
            _logger.LogWarning("Unauthorized attempt to delete user from group.");
            return Unauthorized();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGroups()
        {
            _logger.LogInformation("Fetching all groups for the current user.");
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var groups = await _groupService.GetGroupsAsync(currentUserId);
                _logger.LogInformation("Groups fetched successfully.");
                return Ok(groups);
            }
            _logger.LogWarning("Unauthorized attempt to fetch groups.");
            return Unauthorized();
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteAsync(Guid groupId)
        {
            _logger.LogInformation("Attempting to delete group with ID: {GroupId}", groupId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, groupId))
                {
                    await _groupService.DeleteAsync(groupId);
                    _logger.LogInformation("Group deleted successfully.");
                    return NoContent();
                }
                _logger.LogWarning("Forbidden: User is not the creator of the group.");
                return Forbid();
            }
            _logger.LogWarning("Unauthorized attempt to delete group.");
            return Unauthorized();
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> ReadGroup(Guid groupId)
        {
            _logger.LogInformation("Attempting to fetch group with ID: {GroupId}", groupId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);
                if (!isUserExist)
                {
                    _logger.LogWarning("Forbidden: User does not belong to the group.");
                    return Forbid();
                }
                var group = await _groupService.GetGroupAsync(groupId, currentUserId);
                _logger.LogInformation("Group fetched successfully.");
                return Ok(group);
            }
            _logger.LogWarning("Unauthorized attempt to fetch group.");
            return Unauthorized();
        }

        [HttpGet("users/{groupId}")]
        public async Task<IActionResult> ReadGroupUsers(Guid groupId)
        {
            _logger.LogInformation("Attempting to fetch users of group with ID: {GroupId}", groupId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);
                if (!isUserExist)
                {
                    _logger.LogWarning("Forbidden: User does not belong to the group.");
                    return Forbid();
                }
                var users = await _groupService.GetGroupUsersAsync(groupId);
                _logger.LogInformation("Group users fetched successfully.");
                return Ok(users);
            }
            _logger.LogWarning("Unauthorized attempt to fetch group users.");
            return Unauthorized();
        }

        [HttpGet("image")]
        public IActionResult GetGroupImage(string imagePath)
        {
            _logger.LogInformation("Attempting to fetch group image from path: {ImagePath}", imagePath);
            var fileFullPath = Path.Combine(_folderPath, imagePath);
            if (!System.IO.File.Exists(fileFullPath))
            {
                _logger.LogWarning("Group image not found at path: {ImagePath}", imagePath);
                return NotFound();
            }
            _logger.LogInformation("Group image fetched successfully.");
            return PhysicalFile(fileFullPath, "application/octet-stream");
        }

        [HttpPatch("image/{groupId}")]
        public async Task<IActionResult> UpdateGroupImage(Guid groupId, IFormFile? file)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("Attempting to update group image for group ID: {GroupId}", groupId);

                string profileImagesFolder = Path.Combine(_folderPath, "group-images");

                Directory.CreateDirectory(profileImagesFolder);

                string? oldFile;

                if (!await _groupService.IsUserGroupCreatorAsync(currentUserId, groupId))
                {
                    _logger.LogWarning("Forbidden: User is not the creator of the group.");
                    return Forbid();
                }

                if (file == null || file.Length == 0)
                {
                    oldFile = Directory.GetFiles(Path.Combine(_folderPath, "group-images"), $"{currentUserId}.*")
                        .FirstOrDefault();
                    if (oldFile != null) System.IO.File.Delete(oldFile);
                    await _groupService.UpdateGroupImageAsync(groupId, null);
                    _logger.LogInformation("Group image removed successfully.");
                    return Created();
                }
                else
                {
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        _logger.LogWarning("Image size exceeds limit for group ID: {GroupId}", groupId);
                        return StatusCode(409);
                    }

                    try
                    {
                        using var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
                    }
                    catch (SixLabors.ImageSharp.UnknownImageFormatException ex)
                    {
                        _logger.LogError(ex, "Invalid image format for group ID: {GroupId}", groupId);
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
                _logger.LogInformation("Group image updated successfully for group ID: {GroupId}", groupId);
                return NoContent();
            }
            _logger.LogWarning("Unauthorized attempt to update group image.");
            return Unauthorized();
        }

        [HttpPatch("name")]
        public async Task<IActionResult> UpdateGroupName([FromBody] GroupNameDto groupNameDto)
        {
            _logger.LogInformation("Attempting to update group name for group ID: {GroupId}", groupNameDto.GroupId);
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                if (await _groupService.IsUserGroupCreatorAsync(currentUserId, groupNameDto.GroupId))
                {
                    await _groupService.UpdateGroupNameAsync(groupNameDto);
                    _logger.LogInformation("Group name updated successfully for group ID: {GroupId}", groupNameDto.GroupId);
                    return NoContent();
                }
                _logger.LogWarning("Forbidden: User is not the creator of the group.");
                return Forbid();
            }
            _logger.LogWarning("Unauthorized attempt to update group name.");
            return Unauthorized();
        }
    }
}