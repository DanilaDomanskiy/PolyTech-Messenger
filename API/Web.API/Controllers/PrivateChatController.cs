using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTO_s.PrivateChat;
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

        [HttpGet]
        public async Task<IActionResult> GetChatsName()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var chats = await _privateChatService.GetUserChatsAsync(userId);
            return Ok(chats);
        }

        [HttpPost("{userId}")]
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

        [HttpGet("chatName/{privateChatId}")]
        public async Task<IActionResult> GetUserName(int privateChatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userName = await _privateChatService.GetOtherUserNameAsync(userId, privateChatId);
            return userName == null ? Unauthorized() : Ok(userName);
        }

        [HttpGet("profileImagePath/{privateChatId}")]
        public async Task<IActionResult> GetUserProfileImagePath(int privateChatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userProfileImagePath = await _privateChatService.GetOtherUserProfileImagePathAsync(userId, privateChatId);
            return userProfileImagePath == null ? Unauthorized() : Ok(userProfileImagePath);
        }
    }
}