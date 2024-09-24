using Microsoft.AspNetCore.Mvc;
using Web.API.ViewModels.UserVIewModels;
using Web.Application.DTO_s;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterViewModel model)
        {
            var user = new RegisterUserDTO
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };

            try
            {
                await _userService.RegisterUserAsync(user);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409);
            }
            
            return Created();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthViewModel model)
        {
            var user = new AuthUserDTO
            {
                Email = model.Login,
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
    }
}
