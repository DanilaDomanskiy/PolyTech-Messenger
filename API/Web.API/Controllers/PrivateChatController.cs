using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.ViewModels.PrivateChat;
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
        public async Task<IActionResult> GetChats2UserName()
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

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] int user2Id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var privateChat = new PrivateChatUsersDTO
            {
                User1Id = userId,
                User2Id = user2Id
            };
            await _privateChatService.CreateChatAsync(privateChat);
            return Created();
        }
    }
}