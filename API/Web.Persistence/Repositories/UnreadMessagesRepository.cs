using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class UnreadMessagesRepository : BaseRepository<UnreadMessages>, IUnreadMessagesRepository
    {
        public UnreadMessagesRepository(WebContext context) : base(context)
        {
        }

        public async Task СlearPrivateChatUnreadMessagesAsync(Guid userId, Guid privateChatId)
        {
            var unreadMessages = _context.UnreadMessages
                .AsNoTracking()
                .FirstOrDefault(um => um.UserId == userId && um.PrivateChatId == privateChatId);

            if (unreadMessages != null && unreadMessages.Count != 0)
            {
                unreadMessages.Count = 0;
                _context.UnreadMessages.Update(unreadMessages);
                await _context.SaveChangesAsync();
            }
        }

        public async Task СlearGroupUnreadMessagesAsync(Guid userId, Guid groupId)
        {
            var unreadMessages = _context.UnreadMessages
                .AsNoTracking()
                .FirstOrDefault(um => um.UserId == userId && um.GroupId == groupId);

            if (unreadMessages != null && unreadMessages.Count != 0)
            {
                unreadMessages.Count = 0;
                _context.UnreadMessages.Update(unreadMessages);
                await _context.SaveChangesAsync();
            }
        }
    }
}