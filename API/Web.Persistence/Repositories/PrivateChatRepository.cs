using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class PrivateChatRepository : BaseRepository<PrivateChat>, IPrivateChatRepository
    {
        public PrivateChatRepository(WebContext context) : base(context)
        {
        }

        public override async Task<PrivateChat?> ReadAsync(Guid id)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Include(chat => chat.User1)
                .Include(chat => chat.User2)
                .FirstOrDefaultAsync(chat => chat.Id == id);
        }

        public async Task<IEnumerable<PrivateChat>> GetChatsAsync(Guid userId)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Where(chat => chat.User1Id == userId || chat.User2Id == userId)
                .Include(chat => chat.User1)
                .Include(chat => chat.User2)
                .ToListAsync();
        }

        public async Task<Guid?> GetChatIdIfExistsAsync(Guid user1Id, Guid user2Id)
        {
            var chat = await _context.PrivateChats
                .AsNoTracking()
                .FirstOrDefaultAsync(u => (u.User1Id == user1Id && u.User2Id == user2Id)
                                       || (u.User1Id == user2Id && u.User2Id == user1Id));

            return chat?.Id;
        }
    }
}