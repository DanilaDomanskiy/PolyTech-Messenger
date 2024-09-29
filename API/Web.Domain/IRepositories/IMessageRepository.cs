using Web.Core.Entites;

namespace Web.Core.IRepositories
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int privateChatId);
    }
}