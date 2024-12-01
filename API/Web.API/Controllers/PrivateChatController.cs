using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.PrivateChat;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/chat")]
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

        [HttpGet("{chatId}")]
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
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto createPrivateChatDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var chatId = await _privateChatService.CreateChatAsync(currentUserId, createPrivateChatDto);
                return Ok(chatId);
            }
            return Unauthorized();
        }

        [HttpDelete("empty/{chatId}")]
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