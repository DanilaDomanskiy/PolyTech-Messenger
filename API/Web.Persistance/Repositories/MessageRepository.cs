using Microsoft.EntityFrameworkCore;
using Web.Core.Entites;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(WebContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int privateChatId)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.PrivateChatId == privateChatId)
                .Include(m => m.Sender)
                .ToListAsync();
        }
    }
}
