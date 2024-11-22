using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUnreadMessagesRepository : IBaseRepository<UnreadMessages>
    {
        Task СlearGroupUnreadMessagesAsync(Guid userId, Guid groupId);

        Task СlearPrivateChatUnreadMessagesAsync(Guid userId, Guid privateChatId);
    }
}