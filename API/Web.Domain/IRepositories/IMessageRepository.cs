using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int privateChatId);
    }
}