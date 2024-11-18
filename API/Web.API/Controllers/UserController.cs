using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.User;
using Web.Application.Interfaces.IServices;

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
        [HttpGet("searchByEmail")]
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
        [HttpPut("profileImage")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile file)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                var validContentTypes = new[] { "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp" };
                var contentType = file.ContentType.ToLowerInvariant();

                if (!validExtensions.Contains(extension) || !validContentTypes.Contains(contentType))
                {
                    return StatusCode(409);
                }

                var oldFiles = Directory.GetFiles(Path.Combine(_folderPath, "profile-images"), $"{currentUserId}.*");

                foreach (var oldFile in oldFiles)
                {
                    System.IO.File.Delete(oldFile);
                }

                var path = $"profile-images/{currentUserId}{extension}";
                var filePath = Path.Combine(_folderPath, path);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                await _userService.UpdateProfileAsync(path, currentUserId);

                return NoContent();
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("profileImage")]
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
    }
}