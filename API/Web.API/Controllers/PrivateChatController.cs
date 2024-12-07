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
        private readonly ILogger<PrivateChatController> _logger;

        public PrivateChatController(
            IPrivateChatService privateChatService,
            IMessageService messageService,
            ILogger<PrivateChatController> logger)
        {
            _privateChatService = privateChatService;
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetChats()
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("Fetching all chats for user with ID {UserId}", currentUserId);
                var chats = await _privateChatService.GetChatsAsync(currentUserId);
                _logger.LogInformation("Successfully fetched {ChatCount} chats for user with ID {UserId}", chats.Count(), currentUserId);
                return Ok(chats);
            }
            _logger.LogWarning("Unauthorized access attempt to fetch chats");
            return Unauthorized();
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> ReadChat(Guid chatId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is attempting to read chat with ID {ChatId}", currentUserId, chatId);

                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);
                if (!isUserExist)
                {
                    _logger.LogWarning("User with ID {UserId} tried to read chat with ID {ChatId} but is not a member", currentUserId, chatId);
                    return Forbid();
                }

                var chat = await _privateChatService.GetChatAsync(chatId, currentUserId);
                _logger.LogInformation("Successfully fetched chat with ID {ChatId} for user with ID {UserId}", chatId, currentUserId);
                return Ok(chat);
            }
            _logger.LogWarning("Unauthorized access attempt to read chat with ID {ChatId}", chatId);
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto createPrivateChatDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is creating a new chat", currentUserId);

                var chatId = await _privateChatService.CreateChatAsync(currentUserId, createPrivateChatDto);
                _logger.LogInformation("New chat created successfully with ID {ChatId} for user with ID {UserId}", chatId, currentUserId);

                return Ok(chatId);
            }
            _logger.LogWarning("Unauthorized access attempt to create a new chat");
            return Unauthorized();
        }

        [HttpDelete("empty/{chatId}")]
        public async Task<IActionResult> DeleteChatIfEmpty(Guid chatId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is attempting to delete chat with ID {ChatId} if empty", currentUserId, chatId);

                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);
                if (!isUserExist)
                {
                    _logger.LogWarning("User with ID {UserId} tried to delete chat with ID {ChatId} but is not a member", currentUserId, chatId);
                    return Forbid();
                }

                await _privateChatService.DeleteIfEmptyAsync(chatId);
                _logger.LogInformation("Chat with ID {ChatId} deleted successfully for user with ID {UserId}", chatId, currentUserId);

                return NoContent();
            }
            _logger.LogWarning("Unauthorized access attempt to delete chat with ID {ChatId}", chatId);
            return Unauthorized();
        }
    }
}