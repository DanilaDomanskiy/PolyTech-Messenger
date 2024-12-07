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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IConfiguration configuration, ILogger<UserController> logger)
        {
            _userService = userService;
            _folderPath = configuration["FileStorageSettings:UploadFolderPath"];
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            _logger.LogInformation("Registering a new user with email {Email}", model.Email);

            try
            {
                await _userService.AddUserAsync(model);
                _logger.LogInformation("User with email {Email} registered successfully", model.Email);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User with email {Email} already exists", model.Email);
                return StatusCode(409);
            }

            return Created();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthUserDto user)
        {
            _logger.LogInformation("Attempting to login user with email {Email}", user.Login);

            var token = await _userService.LoginUserAsync(user);

            if (token == null)
            {
                _logger.LogWarning("Failed login attempt for email {Email}", user.Login);
                return StatusCode(409);
            }

            _logger.LogInformation("User with email {Email} logged in successfully", user.Login);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
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
            _logger.LogInformation("Logging out the user");

            if (Request.Cookies.ContainsKey("AppCookie"))
            {
                Response.Cookies.Delete("AppCookie");
                _logger.LogInformation("User logged out and cookie deleted");
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
                _logger.LogInformation("User with ID {UserId} is searching for email {Email}", currentUserId, email);
                var users = await _userService.SearchByEmailAsync(email, currentUserId);
                return Ok(users);
            }
            _logger.LogWarning("Unauthorized access attempt while searching for email {Email}", email);
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("id")]
        public IActionResult GetUserId()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("Retrieving user ID for user with ID {UserId}", currentUserId);
                return Ok(currentUserId);
            }
            _logger.LogWarning("Unauthorized access attempt to get user ID");
            return Unauthorized();
        }

        [Authorize]
        [HttpPatch("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile? file)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is updating profile image", currentUserId);

                string profileImagesFolder = Path.Combine(_folderPath, "profile-images");

                Directory.CreateDirectory(profileImagesFolder);

                string? oldFile;

                if (file == null || file.Length == 0)
                {
                    oldFile = Directory.GetFiles(profileImagesFolder, $"{currentUserId}.*")
                        .FirstOrDefault();
                    if (oldFile != null) System.IO.File.Delete(oldFile);
                    await _userService.UpdateProfileImageAsync(currentUserId, null);
                    _logger.LogInformation("Profile image deleted for user with ID {UserId}", currentUserId);
                    return Created();
                }
                else
                {
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        _logger.LogWarning("File size is too large for user with ID {UserId}", currentUserId);
                        return StatusCode(409);
                    }

                    try
                    {
                        using var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
                    }
                    catch (SixLabors.ImageSharp.UnknownImageFormatException)
                    {
                        _logger.LogWarning("Invalid image format for user with ID {UserId}", currentUserId);
                        return StatusCode(409);
                    }
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var path = $"profile-images/{currentUserId}{extension}";
                var filePath = Path.Combine(_folderPath, path);

                oldFile = Directory.GetFiles(profileImagesFolder, $"{currentUserId}.*")
                    .FirstOrDefault();
                if (oldFile != null) System.IO.File.Delete(oldFile);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                await _userService.UpdateProfileImageAsync(currentUserId, path);
                _logger.LogInformation("Profile image updated successfully for user with ID {UserId}", currentUserId);

                return Created();
            }
            _logger.LogWarning("Unauthorized access attempt to update profile image");
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("profile-image")]
        public IActionResult GetProfileImage(string imagePath)
        {
            var fileFullPath = Path.Combine(_folderPath, imagePath);
            if (!System.IO.File.Exists(fileFullPath))
            {
                _logger.LogWarning("Profile image not found at path {ImagePath}", imagePath);
                return NotFound();
            }

            _logger.LogInformation("Serving profile image from path {ImagePath}", imagePath);
            return PhysicalFile(fileFullPath, "application/octet-stream");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("Retrieving current user information for user with ID {UserId}", currentUserId);
                var user = await _userService.GetUserAsync(currentUserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", currentUserId);
                    return NotFound();
                }
                return Ok(user);
            }
            _logger.LogWarning("Unauthorized access attempt to retrieve user information");
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("not-in-group/{groupId}")]
        public async Task<IActionResult> GetNoGroupUsers(string email, Guid groupId)
        {
            _logger.LogInformation("Retrieving users not in group {GroupId} with email {Email}", groupId, email);
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
                _logger.LogInformation("User with ID {UserId} is updating their name", currentUserId);
                await _userService.UpdateUserNameAsync(currentUserId, userNameDto);
                return NoContent();
            }
            _logger.LogWarning("Unauthorized access attempt to update name");
            return Unauthorized();
        }

        [Authorize]
        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordDto updatePasswordDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is updating their password", currentUserId);

                try
                {
                    await _userService.UpdateUserPasswordAsync(currentUserId, updatePasswordDto);
                    if (Request.Cookies.ContainsKey("AppCookie"))
                    {
                        Response.Cookies.Delete("AppCookie");
                        _logger.LogInformation("User with ID {UserId} logged out and cookie deleted after password update", currentUserId);
                    }
                    return NoContent();
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Failed password update attempt for user with ID {UserId}", currentUserId);
                    return StatusCode(409);
                }
            }
            _logger.LogWarning("Unauthorized access attempt to update password");
            return Unauthorized();
        }
    }
}