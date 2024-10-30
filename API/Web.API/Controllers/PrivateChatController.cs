using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.PrivateChat;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/privateChat")]
    [ApiController]
    public class PrivateChatController : ControllerBase
    {
        private readonly IPrivateChatService _privateChatService;

        public PrivateChatController(IPrivateChatService privateChatService)
        {
            _privateChatService = privateChatService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetChats()
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var chats = await _privateChatService.GetChatsAsync(currentUserId);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(Guid otherUser)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var privateChat = new PrivateChatUsersDto
            {
                User1Id = currentUserId,
                User2Id = otherUser
            };
            var chatId = await _privateChatService.CreateChatAsync(privateChat);
            return Ok(chatId);
        }
    }
}