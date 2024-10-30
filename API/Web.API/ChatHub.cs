using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web.Application.Dto_s.Message;
using Web.Application.Interfaces.IServices;

namespace Web.API
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public ChatHub(
            IMessageService messageService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _messageService = messageService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendPrivateChatMessage(SendMessageDto model)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                await Clients.GroupExcept("pc" + model.PrivateChatId, Context.ConnectionId)
                    .SendAsync("ReceivePrivateChatMessage", userId, model.Content, model.Timestamp);

                var message = new SaveMessageDto
                {
                    Content = model.Content,
                    SenderId = userId,
                    Timestamp = model.Timestamp,
                    PrivateChatId = model.PrivateChatId
                };

                await _messageService.SaveMessageAsync(message);
            }
        }
    }
}