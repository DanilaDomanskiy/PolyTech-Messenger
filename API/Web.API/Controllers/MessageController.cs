﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IPrivateChatService _privateChatService;

        public MessageController(
            IMessageService messageService,
            IHubContext<ChatHub> chatHub,
            IUserService userService,
            IPrivateChatService privateChatService)
        {
            _messageService = messageService;
            _chatHub = chatHub;
            _userService = userService;
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

        [HttpPost]
        public async Task<IActionResult> SendMassege([FromBody] SendMessageDto model)
        {
            var senderId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "userId").Value);
            var userName = await _userService.GetUserNameAsync(senderId);

            if (userName == null)
            {
                return Unauthorized();
            }

            await _chatHub.Clients
                .GroupExcept("pc" + model.PrivateChatId.ToString(), model.ConnectionId)
                .SendAsync("ReceiveMessage", userName, model.Content, model.Timestamp);

            var message = new SaveMessageDto
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