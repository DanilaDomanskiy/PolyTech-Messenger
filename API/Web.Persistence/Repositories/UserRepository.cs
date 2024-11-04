using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WebContext context) : base(context)
        {
        }

        public async Task<User?> ReadAsyncByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>?> ReadAsyncByEmailLetters(string email, Guid id)
        {
            return await _context.Users
                .Where(u => u.Email.StartsWith(email) && u.Id != id)
                .ToListAsync();
        }
    }
}