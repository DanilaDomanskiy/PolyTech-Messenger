using Web.Application.Services.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly IUserConnectionRepository _userConnectionRepository;

        public UserConnectionService(IUserConnectionRepository userConnectionRepository)
        {
            _userConnectionRepository = userConnectionRepository;
        }

        public async Task AddConnectionAsync(Guid userId, string connectionId)
        {
            var userConnection = new UserConnection
            {
                UserId = userId,
                ConnectionId = connectionId
            };
            await _userConnectionRepository.CreateAsync(userConnection);
        }

        public async Task RemoveConnectionAsync(string connectionId)
        {
            await _userConnectionRepository.DeleteConnectionAsync(connectionId);
        }

        public async Task<IEnumerable<string>?> GetConnectionsByChatIdAsync(Guid chatId)
        {
            return await _userConnectionRepository.ReadConnectionsByChatIdAsync(chatId);
        }

        public async Task<IEnumerable<string>?> GetConnectionsByGroupIdAsync(Guid groupId)
        {
            return await _userConnectionRepository.ReadConnectionsByGroupIdAsync(groupId);
        }

        public async Task<IEnumerable<string>?> GetAllConnectionsAsync(Guid userId)
        {
            return await _userConnectionRepository.ReadAllConnectionsAsync(userId);
        }

        public async Task SetActivePrivateChatAsync(string connectionId, Guid? privateChatId)
        {
            await _userConnectionRepository.SetActivePrivateChatAsync(connectionId, privateChatId);
        }

        public async Task SetActiveGroupAsync(string connectionId, Guid? groupId)
        {
            await _userConnectionRepository.SetActiveGroupAsync(connectionId, groupId);
        }
    }
}