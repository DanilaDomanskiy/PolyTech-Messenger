using Microsoft.EntityFrameworkCore;
using Web.Core.Entites;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class PrivateChatRepository : BaseRepository<PrivateChat>, IPrivateChatRepository
    {
        public PrivateChatRepository(WebContext context) : base(context)
        {
        }

        public override async Task<PrivateChat?> ReadAsync(int id)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Include(chat => chat.User1)
                .Include(chat => chat.User2)
                .FirstOrDefaultAsync(chat => chat.Id == id); 
        }

        public async Task<IEnumerable<PrivateChat>> GetChatsAsync(int userId)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Where(chat => chat.User1Id == userId || chat.User2Id == userId)
                .Include(chat => chat.User1)
                .Include(chat => chat.User2)
                .ToListAsync();
        }
    }
}
