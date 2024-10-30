using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesAsync(Guid privateChatId, int page, int pageSize);
    }
}