using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> IsUserExistsAsync(string email);

        Task<User?> ReadAsyncByEmail(string email);

        Task<IEnumerable<User>> ReadAsyncByEmailLetters(string email);
    }
}