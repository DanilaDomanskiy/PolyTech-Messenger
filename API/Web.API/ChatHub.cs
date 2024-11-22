using Microsoft.AspNetCore.SignalR;
using Web.Application.Dto_s.Message;
using Web.Application.Services.Interfaces.IServices;

namespace Web.API
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessageService _messageService;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IPrivateChatService _privateChatService;
        private readonly IUnreadMessagesService _unreadMessagesService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;

        public ChatHub(
            IMessageService messageService,
            IHttpContextAccessor httpContextAccessor,
            IUserConnectionService userConnectionService,
            IPrivateChatService privateChatService,
            IUnreadMessagesService unreadMessagesService,
            IUserService userService,
            IGroupService groupService)
        {
            _messageService = messageService;
            _httpContextAccessor = httpContextAccessor;
            _userConnectionService = userConnectionService;
            _privateChatService = privateChatService;
            _unreadMessagesService = unreadMessagesService;
            _userService = userService;
            _groupService = groupService;
        }

        public async Task JoinPrivateChatAsync(Guid privateChatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "pc" + privateChatId.ToString());
        }

        public async Task LeavePrivateChatAsync(Guid privateChatId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                await _unreadMessagesService.СlearPrivateChatUnreadMessagesAsync(userId, privateChatId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "pc" + privateChatId.ToString());
        }

        public async Task JoinGroupAsync(Guid groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "g" + groupId.ToString());
        }

        public async Task LeaveGroupAsync(Guid groupId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                await _unreadMessagesService.СlearGroupUnreadMessagesAsync(userId, groupId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "g" + groupId.ToString());
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
                if (!await _privateChatService.IsUserExistInChatAsync(userId, model.PrivateChatId)) return;

                var message = new SaveMessageDto
                {
                    Content = model.Content,
                    SenderId = userId,
                    Timestamp = model.Timestamp,
                    PrivateChatId = model.PrivateChatId
                };

                var messageId = await _messageService.SaveMessageAsync(message);

                var dto = new ReceiveChatMessageDto
                {
                    Id = messageId,
                    Content = model.Content,
                    Timestamp = model.Timestamp,
                    SenderId = userId
                };

                await Clients.GroupExcept("pc" + model.PrivateChatId, Context.ConnectionId).SendAsync("ReceivePrivateChatMessage", dto);

                var usersConnections = await _userConnectionService.GetConnectionsByChatIdAsync(model.PrivateChatId);

                if (usersConnections?.Any() == true)
                {
                    foreach (var connection in usersConnections)
                    {
                        if (connection != Context.ConnectionId)
                        {
                            await Clients.Client(connection).SendAsync("ReceivePrivateChatMessages", model.PrivateChatId, dto);
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
                if (!await _groupService.IsUserExistInGroupAsync(userId, model.GroupId)) return;

                var message = new SaveMessageDto
                {
                    Content = model.Content,
                    SenderId = userId,
                    Timestamp = model.Timestamp,
                    GroupId = model.GroupId,
                };

                var messageId = await _messageService.SaveMessageAsync(message);
                var user = await _userService.GetUserAsync(userId);
                var dto = new ReceiveGroupMessageDto
                {
                    Id = messageId,
                    Content = model.Content,
                    Timestamp = model.Timestamp,
                    GroupId = model.GroupId,
                    Sender = new Sender
                    {
                        Id = userId,
                        Name = user?.Name,
                        ProfileImagePath = user?.ProfileImagePath,
                    }
                };

                await Clients.GroupExcept("g" + model.GroupId, Context.ConnectionId).SendAsync("ReceiveGroupMessage", dto);

                var usersConnections = await _userConnectionService.GetConnectionsByGroupIdAsync(model.GroupId);

                if (usersConnections?.Any() == true)
                {
                    foreach (var connection in usersConnections)
                    {
                        if (connection != Context.ConnectionId)
                        {
                            await Clients.Client(connection).SendAsync("ReceiveGroupMessages", model.GroupId, dto);
                        }
                    }
                }

                await _groupService.UpdateUnreadMessagesAsync(model.GroupId, userId);
            }
        }
    }
}