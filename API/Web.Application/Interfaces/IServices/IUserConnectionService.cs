namespace Web.Application.Interfaces.IServices
{
    public interface IUserConnectionService
    {
        Task AddConnectionAsync(Guid userId, string connectionId);

        Task<IEnumerable<string>?> GetAllConnectionsAsync(Guid userId);

        Task<IEnumerable<string>?> GetConnectionsByChatIdAsync(Guid chatId);

        Task RemoveConnectionAsync(string connectionId);
    }
}