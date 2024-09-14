using Web.Core.Entites;

namespace Web.Core.IRepositories
{
    public interface IPrivateChatRepository
    {
        Task AddChatAsync(PrivateChat privateChat);
        Task<PrivateChat?> GetChatAsync(int id);
        Task<IEnumerable<PrivateChat>> GetChatsAsync(int userId);
    }
}
