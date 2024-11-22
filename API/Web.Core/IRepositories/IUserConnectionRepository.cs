using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUserConnectionRepository : IBaseRepository<UserConnection>
    {
        Task<IEnumerable<string>?> ReadConnectionsByChatIdAsync(Guid chatId);

        Task DeleteConnectionAsync(string connectionId);

        Task<IEnumerable<string>?> ReadAllConnectionsAsync(Guid userId);
        Task<IEnumerable<string>?> ReadConnectionsByGroupIdAsync(Guid groupId);
    }
}