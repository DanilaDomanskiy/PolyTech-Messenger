using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IPrivateChatRepository : IBaseRepository<PrivateChat>
    {
        Task<IEnumerable<PrivateChat>?> ReadChatsWithSecondUserAsync(Guid userId);

        Task<Guid?> ReadChatIdIfChatExistsAsync(Guid user1Id, Guid user2Id);

        Task DeleteIfEmptyAsync(Guid chatId);

        Task<PrivateChat?> ReadChatWithSecondUserAsync(Guid chatId, Guid currentUserId);

        Task<bool> IsUserExistInChatAsync(Guid userId, Guid chatId);

        Task UpdateUnreadMessagesAsync(Guid privateChatId, Guid currentUserId);
    }
}