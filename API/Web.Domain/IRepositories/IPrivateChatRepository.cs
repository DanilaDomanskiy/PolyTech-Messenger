using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IPrivateChatRepository : IBaseRepository<PrivateChat>
    {
        Task<IEnumerable<PrivateChat>> GetChatsAsync(int userId);
    }
}