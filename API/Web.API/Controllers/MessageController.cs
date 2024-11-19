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

        public MessageController(
            IMessageService messageService,
            IPrivateChatService privateChatService,
            IGroupService groupService)
        {
            _messageService = messageService;
            _privateChatService = privateChatService;
            _groupService = groupService;
        }

        [HttpGet("byChatId")]
        public async Task<IActionResult> GetChatMessages(Guid chatId, int page = 1, int pageSize = 20)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);

                if (!isUserExist)
                {
                    return Forbid();
                }

                var messages = await _messageService.GetChatMessagesAsync(chatId, currentUserId, page, pageSize);

                return Ok(messages);
            }
            return Unauthorized();
        }

        [HttpGet("byGroupId")]
        public async Task<IActionResult> GetGroupMessages(Guid groupId, int page = 1, int pageSize = 20)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _groupService.IsUserExistInGroupAsync(currentUserId, groupId);

                if (!isUserExist)
                {
                    return Forbid();
                }

                var messages = await _messageService.GetGroupMessagesAsync(groupId, currentUserId, page, pageSize);

                return Ok(messages);
            }
            return Unauthorized();
        }

        [HttpDelete("/{messageId}")]
        public async Task<IActionResult> DeleteAsync(Guid messageId)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                await _messageService.DeleteAsync(messageId, currentUserId);
                return NoContent();
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] SaveMessageDto saveMessageDto)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                saveMessageDto.SenderId = currentUserId;
                await _messageService.SaveMessageAsync(saveMessageDto);
                return NoContent();
            }
            return Unauthorized();
        }
    }
}