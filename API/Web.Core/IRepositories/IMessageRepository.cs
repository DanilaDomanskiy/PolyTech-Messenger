using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<IEnumerable<Message>?> ReadChatMessagesAsync(Guid privateChatId, Guid userId, int page, int pageSize);

        Task<IEnumerable<Message>?> ReadGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize);
    }
}