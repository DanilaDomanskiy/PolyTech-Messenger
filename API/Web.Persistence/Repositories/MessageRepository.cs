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

        public async Task<IEnumerable<Message>?> ReadChatMessagesAsync(Guid privateChatId, Guid userId, int page, int pageSize)
        {
            var unreadMessages = await _context.UnreadMessages
                .FirstOrDefaultAsync(um => um.UserId == userId && um.PrivateChatId == privateChatId);

            if (unreadMessages != null)
            {
                unreadMessages.Count = 0;
                _context.UnreadMessages.Update(unreadMessages);
                await _context.SaveChangesAsync();
            }

            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.PrivateChatId == privateChatId)
                .OrderByDescending(m => m.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>?> ReadGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize)
        {
            var unreadMessages = await _context.UnreadMessages
                .FirstOrDefaultAsync(um => um.UserId == userId && um.GroupId == groupId);

            if (unreadMessages != null)
            {
                unreadMessages.Count = 0;
                _context.UnreadMessages.Update(unreadMessages);
                await _context.SaveChangesAsync();
            }

            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Sender)
                .ToListAsync();
        }
    }
}