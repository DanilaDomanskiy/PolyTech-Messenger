using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Dto_s.Message;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IPrivateChatService _privateChatService;
        private readonly IGroupService _groupService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(
            IMessageService messageService,
            IPrivateChatService privateChatService,
            IGroupService groupService,
            ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _privateChatService = privateChatService;
            _groupService = groupService;
            _logger = logger;
        }

        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetChatMessages(Guid chatId, int page = 1, int pageSize = 20)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is attempting to fetch messages for chat with ID {ChatId}", currentUserId, chatId);

                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);
                if (!isUserExist)
                {
                    _logger.LogWarning("User with ID {UserId} tried to access chat {ChatId} but is not a member.", currentUserId, chatId);
                    return Forbid();
                }

                var messages = await _messageService.GetChatMessagesAsync(chatId, currentUserId, page, pageSize);
                _logger.LogInformation("Successfully fetched {MessageCount} messages for chat with ID {ChatId}", messages.Count(), chatId);

                return Ok(messages);
            }
            _logger.LogWarning("Unauthorized access attempt to fetch messages for chat with ID {ChatId}", chatId);
            return Unauthorized();
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupMessages(Guid groupId, int page = 1, int pageSize = 20)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is attempting to fetch messages for group with ID {GroupId}", currentUserId, groupId);

                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);
                if (!isUserExist)
                {
                    _logger.LogWarning("User with ID {UserId} tried to access group {GroupId} but is not a member.", currentUserId, groupId);
                    return Forbid();
                }

                var messages = await _messageService.GetGroupMessagesAsync(groupId, currentUserId, page, pageSize);
                _logger.LogInformation("Successfully fetched {MessageCount} messages for group with ID {GroupId}", messages.Count(), groupId);

                return Ok(messages);
            }
            _logger.LogWarning("Unauthorized access attempt to fetch messages for group with ID {GroupId}", groupId);
            return Unauthorized();
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteAsync(Guid messageId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is attempting to delete message with ID {MessageId}", currentUserId, messageId);

                await _messageService.DeleteAsync(messageId, currentUserId);
                _logger.LogInformation("Successfully deleted message with ID {MessageId} for user with ID {UserId}", messageId, currentUserId);

                return NoContent();
            }
            _logger.LogWarning("Unauthorized access attempt to delete message with ID {MessageId}", messageId);
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] SaveMessageDto saveMessageDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                _logger.LogInformation("User with ID {UserId} is saving a new message", currentUserId);

                saveMessageDto.SenderId = currentUserId;
                await _messageService.SaveMessageAsync(saveMessageDto);

                _logger.LogInformation("Successfully saved message for user with ID {UserId}", currentUserId);
                return Created();
            }
            _logger.LogWarning("Unauthorized access attempt to save a message");
            return Unauthorized();
        }
    }
}