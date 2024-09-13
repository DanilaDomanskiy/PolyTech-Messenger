using Microsoft.AspNetCore.Mvc;
using Web.API.Models;
using Web.API.ViewModels;
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

            var result = await _userService.RegisterUserAsync(user);
            if (result.IsSuccess)
            {
                return Created();
            }
            return BadRequest(result.Error);
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
            if (token.IsSuccess)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMonths(1)
                };

                Response.Cookies.Append("AppCookie", token.Value, cookieOptions);
                return Ok();
            }
            return BadRequest(token.Error);
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
