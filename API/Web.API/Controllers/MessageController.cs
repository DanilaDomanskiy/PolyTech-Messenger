using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IPrivateChatService _privateChatService;

        public MessageController(
            IMessageService messageService,
            IPrivateChatService privateChatService)
        {
            _messageService = messageService;
            _privateChatService = privateChatService;
        }

        [HttpGet("byChatId")]
        public async Task<IActionResult> GetMessages(Guid chatId, int page = 1, int pageSize = 20)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                var isUserExist = await _privateChatService.IsUserExistInChatAsync(currentUserId, chatId);

                if (!isUserExist)
                {
                    return Forbid();
                }

                var messages = await _messageService.GetMessagesAsync(chatId, currentUserId, page, pageSize);

                return Ok(messages);
            }
            return Unauthorized();
        }
    }
}