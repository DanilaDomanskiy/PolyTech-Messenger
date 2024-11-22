using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> IsUserExistsAsync(string email);

        Task<User?> ReadByEmailAsync(string email);

        Task<IEnumerable<User>?> ReadByEmailLettersAsync(string email, Guid Id);

        Task<IEnumerable<User>?> ReadNoGroupUsersAsync(string email, Guid groupId);
        Task UpdateNameAsync(Guid userId, string name);
        Task UpdatePasswordAsync(Guid currentUserId, string passwordHash);
    }
}