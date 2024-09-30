using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            var user = new RegisterUserDto
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
<<<<<<< HEAD
            return Unauthorized();
=======
            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMonths(6)
            };

            Response.Cookies.Append("AppCookie", token, cookieOptions);
            return Ok();
>>>>>>> 061ff09db845020465bc2645cbceecd27087d3ec
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
