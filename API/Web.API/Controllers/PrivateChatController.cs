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
        private readonly IMessageService _messageService;

        public PrivateChatController(
            IPrivateChatService privateChatService,
            IMessageService messageService)
        {
            _privateChatService = privateChatService;
            _messageService = messageService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetChats()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var chats = await _privateChatService.GetChatsAsync(currentUserId);
                return Ok(chats);
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> ReadChat(Guid chatId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);
                if (!isUserExist)
                {
                    return Forbid();
                }
                var chat = await _privateChatService.GetChatAsync(chatId, currentUserId);
                return Ok(chat);
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] Guid otherUserId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var privateChat = new PrivateChatUsersDto
                {
                    User1Id = currentUserId,
                    User2Id = otherUserId
                };
                var chatId = await _privateChatService.CreateChatAsync(privateChat);
                return Ok(chatId);
            }
            return Unauthorized();
        }

        [HttpDelete("empty")]
        public async Task<IActionResult> DeleteChatIfEmpty(Guid chatId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);

                if (!isUserExist)
                {
                    return Forbid();
                }

                await _privateChatService.DeleteIfEmptyAsync(chatId);

                return NoContent();
            }
            return Unauthorized();
        }
    }
}