using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTO_s.PrivateChat;
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
        public async Task<IActionResult> GetChatsName()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var chats = await _privateChatService.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] int user2Id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var privateChat = new PrivateChatUsersDto
            {
                User1Id = userId,
                User2Id = user2Id
            };
            await _privateChatService.CreateChatAsync(privateChat);
            return Created();
        }

        [HttpGet("chatName/{privateChatId}")]
        public async Task<IActionResult> GetUserName(int privateChatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userName = await _privateChatService.GetOtherUserNameAsync(userId, privateChatId);
            return userName == null ? Unauthorized() : Ok(userName);
        }
    }
}