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

        public async Task<IEnumerable<PrivateChat>?> ReadChatsWithSecondUserAsync(Guid userId)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Where(pc => pc.Users.Any(user => user.Id == userId))
                .Select(pc => new PrivateChat
                {
                    Id = pc.Id,
                    Users = pc.Users.Where(user => user.Id != userId).ToList(),
                    Messages = pc.Messages.OrderByDescending(m => m.Timestamp).Take(1).ToList(),
                    UnreadMessages = pc.UnreadMessages.Where(um => um.UserId == userId).ToList()
                })
                .ToListAsync();
        }

        public async Task<Guid?> ReadChatIdIfChatExistsAsync(Guid user1Id, Guid user2Id)
        {
            var chat = await _context.PrivateChats
                .AsNoTracking()
                .Where(pc => pc.Users.Any(u => u.Id == user1Id) && pc.Users.Any(u => u.Id == user2Id))
                .FirstOrDefaultAsync();

            return chat?.Id;
        }

        public async Task DeleteIfEmptyAsync(Guid chatId)
        {
            var chat = await _context.PrivateChats
                .Where(pc => pc.Id == chatId && !pc.Messages.Any())
                .FirstOrDefaultAsync();

            if (chat != null)
            {
                _context.PrivateChats.Remove(chat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PrivateChat?> ReadChatWithSecondUserAsync(Guid chatId, Guid currentUserId)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Where(pc => pc.Id == chatId)
                .Select(pc => new PrivateChat
                {
                    Id = pc.Id,
                    Users = pc.Users.Where(user => user.Id != currentUserId).ToList(),
                    Messages = pc.Messages.OrderByDescending(m => m.Timestamp).Take(1).ToList(),
                    UnreadMessages = pc.UnreadMessages
                        .Where(um => um.UserId == currentUserId)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserExistInChatAsync(Guid userId, Guid chatId)
        {
            return await _context.PrivateChats
                .AsNoTracking()
                .Where(chat => chat.Id == chatId)
                .AnyAsync(chat => chat.Users.Any(user => user.Id == userId));
        }

        public async Task UpdateUnreadMessagesAsync(Guid privateChatId, Guid currentUserId)
        {
            var otherUserId = await _context.Users
                .Where(user => user.PrivateChats.Any(pc => pc.Id == privateChatId) && user.Id != currentUserId)
                .Select(user => user.Id)
                .FirstOrDefaultAsync();

            if (otherUserId == Guid.Empty)
                return;

            var unreadMessage = await _context.UnreadMessages
                .FirstOrDefaultAsync(um => um.UserId == otherUserId && um.PrivateChatId == privateChatId);

            if (unreadMessage == null)
            {
                await _context.UnreadMessages.AddAsync(new UnreadMessages
                {
                    UserId = otherUserId,
                    PrivateChatId = privateChatId,
                    Count = 1
                });
            }
            else
            {
                unreadMessage.Count++;
                _context.UnreadMessages.Update(unreadMessage);
            }

            await _context.SaveChangesAsync();
        }
    }
}