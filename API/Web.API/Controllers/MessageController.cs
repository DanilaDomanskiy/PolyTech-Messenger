using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Application.DTO_s.Message;
using Web.Application.Interfaces;
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
        private readonly IPrivateChatService _privateChatService;
        private readonly IEncryptionService _encryptionService;

        public MessageController(
            IMessageService messageService,
            IHubContext<ChatHub> chatHub,
            IUserService userService,
            IPrivateChatService privateChatService,
            IEncryptionService encryptionService)
        {
            _messageService = messageService;
            _chatHub = chatHub;
            _userService = userService;
            _privateChatService = privateChatService;
            _encryptionService = encryptionService;
        }

        [HttpGet("byChatId{chatId}")]
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

        [HttpPost]
        public async Task<IActionResult> SendMassege([FromBody] SendMessageDto model)
        {
            var senderId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userName = await _userService.GetUserNameAsync(senderId);

            if (userName == null)
            {
                return Unauthorized();
            }

            if (model.PrivateChatId.HasValue)
            {
                var privateChatUsers = await _privateChatService.GetChatUsersAsync(model.PrivateChatId.Value);

                if (privateChatUsers == null ||
                    (privateChatUsers.User1Id != senderId && 
                    privateChatUsers.User2Id != senderId))
                {
                    return Unauthorized();
                }

                // await _chatHub.Clients
                //    .GroupExcept("pc" + model.PrivateChatId.Value, model.ConnectionId)
                //    .SendAsync("ReceiveMessage", userName, model.Content, model.Timestamp);
            }
            else if (model.GroupId.HasValue)
            {
                throw new NotImplementedException();
            }

            var message = new SaveMessageDto
            {
                Content = _encryptionService.Encrypt(model.Content),
                SenderId = senderId,
                Timestamp = model.Timestamp,
                GroupId = model.GroupId,
                PrivateChatId = model.PrivateChatId
            };

            await _messageService.SaveMessageAsync(message);

            return Ok();
        }
    }
}