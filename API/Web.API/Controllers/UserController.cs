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
                await _userService.RegisterUserAsync(model);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409);
            }

            return Created();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthUserDto model)
        {
            var user = new AuthUserDto
            {
                Login = model.Login,
                Password = model.Password
            };

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
            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("AppCookie"))
            {
                Response.Cookies.Delete("AppCookie");
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("searchUser/{email}")]
        public async Task<IActionResult> SearchUsersByEmail(string email)
        {
            var users = await _userService.SearchByEmailAsync(email);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("getUserId")]
        public IActionResult GetUserId()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            return Ok(userId);
        }

        [Authorize]
        [HttpPut("updateProfileImage")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var validContentTypes = new[] { "image/jpg", "image/jpeg", "image/png", "image/gif", "image/bmp" };
            var contentType = file.ContentType.ToLowerInvariant();

            if (!validExtensions.Contains(extension) || !validContentTypes.Contains(contentType))
            {
                return StatusCode(409);
            }

            var oldFiles = Directory.GetFiles(Path.Combine(_folderPath, "profile-images"), $"{userId}.*");

            foreach (var oldFile in oldFiles)
            {
                System.IO.File.Delete(oldFile);
            }

            var path = $"profile-images/{userId}{extension}";
            var filePath = Path.Combine(_folderPath, path);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            await _userService.UpdateProfileImageAsync(path, userId);

            return NoContent();
        }

        [Authorize]
        [HttpGet("getProfileImage")]
        public async Task<IActionResult> GetProfileImage(string imagePath)
        {
            var fileFullPath = Path.Combine(_folderPath, imagePath);
            if (!System.IO.File.Exists(fileFullPath))
            {
                return NotFound();
            }
            return PhysicalFile(fileFullPath, "application/octet-stream");
        }
    }
}