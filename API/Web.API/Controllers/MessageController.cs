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

        [HttpGet("byChatId/{chatId}")]
        public async Task<IActionResult> GetMessagesByChatId(int chatId)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var isUserExist = await _privateChatService.IsUserExistInChatAsync(userId, chatId);

            if (!isUserExist)
            {
                return Unauthorized();
            }

            var messages = await _messageService.GetMessagesByChatIdAsync(chatId, userId);

            return Ok(messages);
        }
    }
}