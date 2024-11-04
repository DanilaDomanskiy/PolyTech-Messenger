using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(WebContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>?> GetMessagesAsync(Guid privateChatId, int page, int pageSize)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.PrivateChatId == privateChatId)
                .OrderByDescending(m => m.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}