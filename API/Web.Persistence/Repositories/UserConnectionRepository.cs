using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class UserConnectionRepository : BaseRepository<UserConnection>, IUserConnectionRepository
    {
        public UserConnectionRepository(WebContext context) : base(context)
        {
        }

        public override async Task<Guid> CreateAsync(UserConnection userConnection)
        {
            var user = await _context.Users.FindAsync(userConnection.UserId);
            await _context.UserConnections.AddAsync(userConnection);
            if (user != null && !user.IsActive) user.IsActive = true;
            await _context.SaveChangesAsync();
            return userConnection.Id;
        }

        public async Task<IEnumerable<string>?> ReadConnectionsByChatIdAsync(Guid chatId)
        {
            return await _context.UserConnections
                .Where(
                uc => uc.User.IsActive &&
                uc.User.PrivateChats.Any(pc => pc.Id == chatId) &&
                uc.ActivePrivateChatId != chatId)
                .Select(uc => uc.ConnectionId)
                .ToListAsync();
        }

        public async Task DeleteConnectionAsync(string connectionId)
        {
            var connection = await _context.UserConnections
                .Include(uc => uc.User)
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);

            if (connection == null)
            {
                return;
            }

            var user = connection.User;

            _context.UserConnections.Remove(connection);

            var hasOtherConnections = await _context.UserConnections
                .AnyAsync(uc => uc.UserId == user.Id && uc.ConnectionId != connectionId);

            if (!hasOtherConnections)
            {
                user.IsActive = false;
                user.LastActive = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>?> ReadAllConnectionsAsync(Guid userId)
        {
            return await _context.UserConnections
                .Where(uc => uc.User.IsActive && uc.UserId != userId &&
                             (uc.User.PrivateChats.Any(pc => pc.Users.Any(u => u.Id == userId)) ||
                              uc.User.Groups.Any(g => g.Users.Any(u => u.Id == userId))))
                .Select(uc => uc.ConnectionId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>?> ReadConnectionsByGroupIdAsync(Guid groupId)
        {
            return await _context.UserConnections
                .Where(
                uc => uc.User.IsActive &&
                uc.User.Groups.Any(g => g.Id == groupId) &&
                uc.ActiveGroupId != groupId)
                .Select(uc => uc.ConnectionId)
                .ToListAsync();
        }

        public async Task SetActivePrivateChatAsync(string connectionId, Guid? privateChatId)
        {
            var connection = await _context.UserConnections
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);

            if (connection != null)
            {
                connection.ActivePrivateChatId = privateChatId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetActiveGroupAsync(string connectionId, Guid? groupId)
        {
            var connection = await _context.UserConnections
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);

            if (connection != null)
            {
                connection.ActiveGroupId = groupId;
                await _context.SaveChangesAsync();
            }
        }
    }
}