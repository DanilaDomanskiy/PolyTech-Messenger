using Microsoft.AspNetCore.SignalR;
using Web.Application.Dto_s.Message;
using Web.Application.Interfaces.IServices;

namespace Web.API
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageService _messageService;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IPrivateChatService _privateChatService;
        private readonly IUnreadMessagesService _unreadMessagesService;

        public ChatHub(
            IMessageService messageService,
            IHttpContextAccessor httpContextAccessor,
            IUserConnectionService userConnectionService,
            IPrivateChatService privateChatService,
            IUnreadMessagesService unreadMessagesService)
        {
            _messageService = messageService;
            _httpContextAccessor = httpContextAccessor;
            _userConnectionService = userConnectionService;
            _privateChatService = privateChatService;
            _unreadMessagesService = unreadMessagesService;
        }

        public async Task JoinAsync(string chatName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }

        public async Task LeaveAsync(string chatName)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                string guidString = chatName.Replace("pc", "");
                var guid = Guid.Parse(guidString);
                await _unreadMessagesService.СlearUnreadMessagesAsync(userId, guid, null);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatName);
        }

        public async Task ConnectAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                await _userConnectionService.AddConnectionAsync(userId, Context.ConnectionId);
                await SendIsActiveUser(userId, true);
            }
        }

        public async Task DisconnectAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                await _userConnectionService.RemoveConnectionAsync(Context.ConnectionId);
                await SendIsActiveUser(userId, false);
            }
        }

        private async Task SendIsActiveUser(Guid userId, bool isActive)
        {
            var connections = await _userConnectionService.GetAllConnectionsAsync(userId);

            if (connections?.Any() == true)
            {
                foreach (var connection in connections)
                {
                    await Clients.Client(connection).SendAsync("IsActiveUser", userId, isActive);
                }
            }
        }

        public async Task SendPrivateChatMessageAsync(SendChatMessageDto model)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                var message = new SaveMessageDto
                {
                    Content = model.Content,
                    SenderId = userId,
                    Timestamp = model.Timestamp,
                    PrivateChatId = model.PrivateChatId
                };

                var messageId = await _messageService.SaveMessageAsync(message);

                var privateChat = "pc" + model.PrivateChatId;

                await Clients.GroupExcept(privateChat, Context.ConnectionId)
                    .SendAsync("ReceiveMessage", userId, model.Content, model.Timestamp);

                var usersConnections = await _userConnectionService.GetConnectionsByChatIdAsync(model.PrivateChatId);

                if (usersConnections?.Any() == true)
                {
                    foreach (var connection in usersConnections)
                    {
                        if (connection != Context.ConnectionId)
                        {
                            await Clients.Client(connection)
                                .SendAsync("ReceiveMessages", userId, model.Content, model.Timestamp, model.PrivateChatId);
                        }
                    }
                }

                await _privateChatService.UpdateUnreadMessagesAsync(model.PrivateChatId, userId);
            }
        }

        public async Task SendGroupMessageAsync(SendGroupMessageDto model)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
            }
        }
    }
}