using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IPrivateChatRepository : IBaseRepository<PrivateChat>
    {
        Task<IEnumerable<PrivateChat>?> GetChatsAsync(Guid userId);

        Task<Guid?> GetChatIdIfExistsAsync(Guid user1Id, Guid user2Id);
    }
}