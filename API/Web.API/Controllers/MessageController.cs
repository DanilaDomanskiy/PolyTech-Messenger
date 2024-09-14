using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTO_s.Message;
using Web.Application.Interfaces.IServices;

namespace Web.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IPrivateChatService _privateChatService;
        public MessageController(IMessageService messageService, IPrivateChatService privateChatService)
        {
            _messageService = messageService;
            _privateChatService = privateChatService;
        }

        [HttpGet("byChatId/{id}")]
        public async Task<IActionResult> GetMessagesByChatId(int id)
        {
            var users = await _privateChatService.GetChatUsersAsync(id);
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);

            if (users == null ||
                userId != users.User1Id ||
                userId != users.User2Id)
            {
                return NotFound();
            }

            var messages = await _messageService.GetMessagesByChatIdAsync(id);
            return Ok(messages.Select(message => new ReadMessageViewModel
            {
                Content = message.Content,
                SenderName = message.SenderName,
                Timestamp = message.Timestamp
            }));
        }

        [HttpPost]
        public async Task<IActionResult> SaveMassege([FromBody] SaveMessageViewModel model)
        {
            var message = new SaveMessageDTO
            {
                Content = model.Content,
                SenderId = model.SenderId,
                Timestamp = model.Timestamp,
                GroupId = model?.GroupId,
                PrivateChatId = model?.PrivateChatId
            };

            await _messageService.SaveMessageAsync(message);
            return Created();
        }
    }
}
