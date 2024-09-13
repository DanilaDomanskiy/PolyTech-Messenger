using Web.Core.Entites;

namespace Web.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<bool> IsUserExists(string email);
        Task<User?> ReadAsyncByEmail(string email);
        Task<User?> ReadAsyncById(int id);
    }
}