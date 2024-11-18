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

        public async Task СlearUnreadMessagesAsync(Guid userId, Guid? privateChatId = null, Guid? groupId = null)
        {
            var unreadMessages = _context.UnreadMessages
                .AsNoTracking()
                .FirstOrDefault(um => um.UserId == userId
                && (privateChatId != null
                ? um.PrivateChatId == privateChatId
                : um.GroupId == groupId));

            if (unreadMessages != null && unreadMessages.Count != 0)
            {
                unreadMessages.Count = 0;
                _context.UnreadMessages.Update(unreadMessages);
                await _context.SaveChangesAsync();
            }
        }
    }
}