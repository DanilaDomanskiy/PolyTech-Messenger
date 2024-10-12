using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.User;
using Web.Application.DTO_s.User;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsersByEmail(string email)
        {
            var users = await _userService.SearchByEmailAsync(email);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("userId")]
        public IActionResult SearchUsersByEmail()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            return Ok(userId);
        }

        [Authorize]
        [HttpPost("uploadProfileImage")]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile file)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);

            var profileImageDto = new ProfileImageDto
            {
                Content = await ConvertToByteArray(file),
                UserId = userId,
                Extension = Path.GetExtension(file.FileName).TrimStart('.')
            };

            await _userService.UpdateProfileImageAsync(profileImageDto);

            return Created();
        }

        private async Task<byte[]> ConvertToByteArray(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}