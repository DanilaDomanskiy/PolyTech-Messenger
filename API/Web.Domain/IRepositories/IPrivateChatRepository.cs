using Web.Core.Entites;

namespace Web.Core.IRepositories
{
    public interface IPrivateChatRepository : IBaseRepository<PrivateChat>
    {
        Task<PrivateChat?> GetChatAsync(int id);
        Task<IEnumerable<PrivateChat>> GetChatsAsync(int userId);
    }
}
