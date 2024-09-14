using Microsoft.EntityFrameworkCore;
using Web.Core.Entites;

namespace Web.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly WebContext _context;

        public MessageRepository(WebContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int privateChatId)
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.PrivateChatId == privateChatId)
                .Include(m => m.Sender)
                .ToListAsync();
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}
