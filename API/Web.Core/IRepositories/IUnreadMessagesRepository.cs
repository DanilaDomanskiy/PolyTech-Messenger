using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUnreadMessagesRepository : IBaseRepository<UnreadMessages>
    {
        Task СlearUnreadMessagesAsync(Guid userId, Guid? privateChatId = null, Guid? groupId = null);
    }
}