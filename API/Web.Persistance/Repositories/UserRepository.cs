using Microsoft.EntityFrameworkCore;
using Web.Core.Entites;

namespace Web.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WebContext _context;

        public UserRepository(WebContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> ReadAsyncByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> ReadAsyncById(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        }
    }
}
