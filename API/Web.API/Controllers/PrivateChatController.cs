using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.ViewModels.PrivateChat;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateChatController : ControllerBase
    {
        private readonly IPrivateChatService _privateChatService;

        public PrivateChatController(IPrivateChatService privateChatService)
        {
            _privateChatService = privateChatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsUser()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var chats = await _privateChatService.GetUserChatsAsync(userId);
            return Ok(chats.Select(chat => new PrivateChatUserViewModel
            {
                PrivateChatId = chat.PrivateChatId,
                UserName = chat.UserName,
                UserEmail = chat.UserEmail
            }));
        }
    }
}