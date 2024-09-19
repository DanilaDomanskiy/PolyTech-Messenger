using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IUserService _userService;

        public MessageController(
            IMessageService messageService,
            IHubContext<ChatHub> chatHub,
            IUserService userService)
        {
            _messageService = messageService;
            _chatHub = chatHub;
            _userService = userService;
        }

        [HttpGet("byChatId/{id}")]
        public async Task<IActionResult> GetMessagesByChatId(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var messages = await _messageService.GetMessagesByChatIdAsync(id);
            return Ok(messages.Select(message => new ReadMessageViewModel
            {
                Content = message.Content,
                SenderName = message.SenderName,
                Timestamp = message.Timestamp,
                IsSender = message.SenderId == userId
            }));
        }

        [HttpPost]
        public async Task<IActionResult> SendMassege([FromBody] SendMessageViewModel model)
        {
            var senderId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var user = await _userService.GetUserAsync(senderId);

            if (user == null)
            {
                return Unauthorized();
            }

            await _chatHub.Clients
                .GroupExcept("pc" + model.PrivateChatId.ToString(), model.ConnectionId)
                .SendAsync("ReceiveMessage", user.Name, model.Content, model.Timestamp);

            var message = new SaveMessageDTO
            {
                Content = model.Content,
                SenderId = senderId,
                Timestamp = model.Timestamp,
                GroupId = model?.GroupId,
                PrivateChatId = model?.PrivateChatId
            };

            await _messageService.SaveMessageAsync(message);

            return Ok();
        }
    }
}