using Web.Core.Entites;

namespace Web.Persistence.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int privateChatId);
        Task SaveMessageAsync(Message message);
    }
}