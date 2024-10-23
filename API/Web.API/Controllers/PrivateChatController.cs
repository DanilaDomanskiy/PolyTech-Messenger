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

        [HttpGet("getChats")]
        public async Task<IActionResult> GetChats()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var chats = await _privateChatService.GetChatsItemsAsync(userId);
            return Ok(chats);
        }

        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateChat(int userId)
        {
            var currentUserId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var privateChat = new PrivateChatUsersDto
            {
                User1Id = currentUserId,
                User2Id = userId
            };
            await _privateChatService.CreateChatAsync(privateChat);
            return Created();
        }

        [HttpGet("getChatName/{privateChatId}")]
        public async Task<IActionResult> GetChatName(int privateChatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userName = await _privateChatService.GetOtherUserNameAsync(userId, privateChatId);
            return userName == null ? Unauthorized() : Ok(userName);
        }

        [HttpGet("getChatImagePath/{privateChatId}")]
        public async Task<IActionResult> GetChatImagePath(int privateChatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userProfileImagePath = await _privateChatService
                .GetOtherUserProfileImagePathAsync(userId, privateChatId);
            return userProfileImagePath == null ? Unauthorized() : Ok(userProfileImagePath);
        }
    }
}