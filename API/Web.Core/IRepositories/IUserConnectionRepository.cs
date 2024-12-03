using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUserConnectionRepository : IBaseRepository<UserConnection>
    {
        Task<IEnumerable<string>?> ReadConnectionsByChatIdAsync(Guid chatId);

        Task DeleteConnectionAsync(string connectionId);

        Task<IEnumerable<string>?> ReadAllConnectionsAsync(Guid userId);

        Task<IEnumerable<string>?> ReadConnectionsByGroupIdAsync(Guid groupId);

        Task SetActiveGroupAsync(string connectionId, Guid? groupId);

        Task SetActivePrivateChatAsync(string connectionId, Guid? privateChatId);
    }
}