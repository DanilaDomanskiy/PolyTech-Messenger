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
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(
            IMessageService messageService,
            IHttpContextAccessor httpContextAccessor,
            IUserConnectionService userConnectionService,
            IPrivateChatService privateChatService,
            IUserService userService,
            IGroupService groupService,
            ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _httpContextAccessor = httpContextAccessor;
            _userConnectionService = userConnectionService;
            _privateChatService = privateChatService;
            _userService = userService;
            _groupService = groupService;
            _logger = logger;
        }

        public async Task JoinPrivateChatAsync(Guid privateChatId)
        {
            _logger.LogInformation("User {UserId} is attempting to join private chat {PrivateChatId}", Context.UserIdentifier, privateChatId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "pc" + privateChatId.ToString());
            await _userConnectionService.SetActivePrivateChatAsync(Context.ConnectionId, privateChatId);

            _logger.LogInformation("User {UserId} has joined private chat {PrivateChatId}", Context.UserIdentifier, privateChatId);
        }

        public async Task LeavePrivateChatAsync(Guid privateChatId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is leaving private chat {PrivateChatId}", userId, privateChatId);
                await _userConnectionService.SetActivePrivateChatAsync(Context.ConnectionId, null);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "pc" + privateChatId.ToString());
            _logger.LogInformation("User {UserId} has left private chat {PrivateChatId}", userId, privateChatId);
        }

        public async Task JoinGroupAsync(Guid groupId)
        {
            _logger.LogInformation("User {UserId} is attempting to join group {GroupId}", Context.UserIdentifier, groupId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "g" + groupId.ToString());
            await _userConnectionService.SetActiveGroupAsync(Context.ConnectionId, groupId);

            _logger.LogInformation("User {UserId} has joined group {GroupId}", Context.UserIdentifier, groupId);
        }

        public async Task LeaveGroupAsync(Guid groupId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is leaving group {GroupId}", userId, groupId);
                await _userConnectionService.SetActiveGroupAsync(Context.ConnectionId, null);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "g" + groupId.ToString());
            _logger.LogInformation("User {UserId} has left group {GroupId}", userId, groupId);
        }

        public async Task ConnectAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is connecting", userId);
                await _userConnectionService.AddConnectionAsync(userId, Context.ConnectionId);
                await SendIsActiveUser(userId, true);
                _logger.LogInformation("User {UserId} has connected", userId);
            }
        }

        public async Task DisconnectAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is disconnecting", userId);
                await _userConnectionService.RemoveConnectionAsync(Context.ConnectionId);
                await SendIsActiveUser(userId, false);
                _logger.LogInformation("User {UserId} has disconnected", userId);
            }
        }

        private async Task SendIsActiveUser(Guid userId, bool isActive)
        {
            _logger.LogInformation("Updating activity status for user {UserId} to {IsActive}", userId, isActive);

            var connections = await _userConnectionService.GetAllConnectionsAsync(userId);
            if (connections?.Any() == true)
            {
                foreach (var connection in connections)
                {
                    await Clients.Client(connection).SendAsync("IsActiveUser", userId, isActive);
                }
            }

            _logger.LogInformation("Successfully updated activity status for user {UserId} to {IsActive}", userId, isActive);
        }

        public async Task SendPrivateChatMessageAsync(SendChatMessageDto model)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is sending a message to private chat {PrivateChatId}", userId, model.PrivateChatId);

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

                await Clients.Group("pc" + model.PrivateChatId).SendAsync("ReceivePrivateChatMessage", dto);

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
                _logger.LogInformation("Message sent to private chat {PrivateChatId} by user {UserId}", model.PrivateChatId, userId);
            }
        }

        public async Task SendGroupMessageAsync(SendGroupMessageDto model)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                _logger.LogInformation("User {UserId} is sending a message to group {GroupId}", userId, model.GroupId);

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

                await Clients.Group("g" + model.GroupId).SendAsync("ReceiveGroupMessage", dto);

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
                _logger.LogInformation("Message sent to group {GroupId} by user {UserId}", model.GroupId, userId);
            }
        }
    }
}