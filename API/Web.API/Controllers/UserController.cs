using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.User;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _folderPath;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _folderPath = configuration["FileStorageSettings:UploadFolderPath"];
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            try
            {
                await _userService.AddUserAsync(model);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409);
            }

            return Created();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthUserDto user)
        {
            var token = await _userService.LoginUserAsync(user);

            if (token == null)
            {
                return StatusCode(409);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMonths(6)
            };

            Response.Cookies.Append("AppCookie", token, cookieOptions);
            return NoContent();
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("AppCookie"))
            {
                Response.Cookies.Delete("AppCookie");
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchByEmail(string email)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var users = await _userService.SearchByEmailAsync(email, currentUserId);
                return Ok(users);
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("id")]
        public IActionResult GetUserId()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                return Ok(currentUserId);
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPatch("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile? file)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                string? oldFile;

                if (file == null || file.Length == 0)
                {
                    oldFile = Directory.GetFiles(Path.Combine(_folderPath, "profile-images"), $"{currentUserId}.*")
                        .FirstOrDefault();
                    if (oldFile != null) System.IO.File.Delete(oldFile);
                    await _userService.UpdateProfileImageAsync(currentUserId, null);
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
                var path = $"profile-images/{currentUserId}{extension}";
                var filePath = Path.Combine(_folderPath, path);

                oldFile = Directory.GetFiles(Path.Combine(_folderPath, "profile-images"), $"{currentUserId}.*")
                    .FirstOrDefault();
                if (oldFile != null) System.IO.File.Delete(oldFile);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                await _userService.UpdateProfileImageAsync(currentUserId, path);

                return Created();
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("profile-image")]
        public IActionResult GetProfileImage(string imagePath)
        {
            var fileFullPath = Path.Combine(_folderPath, imagePath);
            if (!System.IO.File.Exists(fileFullPath))
            {
                return NotFound();
            }
            return PhysicalFile(fileFullPath, "application/octet-stream");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var user = await _userService.GetUserAsync(currentUserId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("not-in-group/{groupId}")]
        public async Task<IActionResult> GetNoGroupUsers(string email, Guid groupId)
        {
            var users = await _userService.GetNoGroupUsersAsync(email, groupId);
            return Ok(users);
        }

        [Authorize]
        [HttpPatch("name")]
        public async Task<IActionResult> UpdateName([FromBody] UserNameDto userNameDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                await _userService.UpdateUserNameAsync(currentUserId, userNameDto);
                return NoContent();
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordDto updatePasswordDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                try
                {
                    await _userService.UpdateUserPasswordAsync(currentUserId, updatePasswordDto);
                    if (Request.Cookies.ContainsKey("AppCookie"))
                    {
                        Response.Cookies.Delete("AppCookie");
                    }
                    return NoContent();
                }
                catch (InvalidOperationException)
                {
                    return StatusCode(409);
                }
            }
            return Unauthorized();
        }
    }
}