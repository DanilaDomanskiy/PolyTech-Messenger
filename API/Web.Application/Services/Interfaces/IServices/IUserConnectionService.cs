namespace Web.Application.Services.Interfaces.IServices
{
    public interface IUserConnectionService
    {
        Task AddConnectionAsync(Guid userId, string connectionId);

        Task<IEnumerable<string>?> GetAllConnectionsAsync(Guid userId);

        Task<IEnumerable<string>?> GetConnectionsByChatIdAsync(Guid chatId);
        Task<IEnumerable<string>?> GetConnectionsByGroupIdAsync(Guid groupId);
        Task RemoveConnectionAsync(string connectionId);
    }
}