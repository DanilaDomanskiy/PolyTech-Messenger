using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        public GroupRepository(WebContext context) : base(context)
        {
        }

        public override async Task<Guid> CreateAsync(Group group)
        {
            var user = await _context.Users.FindAsync(group.CreatorId);
            if (user != null)
            {
                group.Users ??= new List<User>();
                group.Users.Add(user);
            }

            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
            return group.Id;
        }

        public async Task AddUserToGroupAsync(Guid userId, Guid groupId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return;

            var group = await _context.Groups
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null || group.Users.Any(u => u.Id == userId))
            {
                return;
            }
            group.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>?> ReadGroupsAsync(Guid userId)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(g => g.Users.Any(user => user.Id == userId))
                .Select(group => new Group
                {
                    Id = group.Id,
                    Name = group.Name,
                    ImagePath = group.ImagePath,
                    CreatorId = group.CreatorId,
                    UnreadMessages = group.UnreadMessages
                        .Where(um => um.UserId == userId)
                        .ToList(),
                    Messages = group.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .Take(1)
                        .Select(m => new Message
                        {
                            Content = m.Content,
                            Timestamp = m.Timestamp,
                            Sender = new User
                            {
                                Id = m.Sender.Id,
                                Name = m.Sender.Name
                            }
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId)
        {
            return await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => g.CreatorId)
                .FirstOrDefaultAsync() == userId;
        }

        public async Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(group => group.Id == groupId)
                .AnyAsync(group => group.Users.Any(user => user.Id == userId));
        }

        public async Task UpdateUnreadMessagesAsync(Guid groupId, Guid userId)
        {
            var otherUsersIds = await _context.Users
                .Where(user => user.Groups.Any(g => g.Id == groupId) && user.Id != userId)
                .Select(user => user.Id)
                .ToListAsync();

            if (otherUsersIds.Count == 0) return;

            var existingUnreadMessages = await _context.UnreadMessages
                .Where(um => um.GroupId == groupId && otherUsersIds.Contains(um.UserId))
                .ToDictionaryAsync(um => um.UserId);

            var newUnreadMessages = new List<UnreadMessages>();
            foreach (var otherUserId in otherUsersIds)
            {
                if (existingUnreadMessages.TryGetValue(otherUserId, out var unreadMessage))
                {
                    unreadMessage.Count++;
                }
                else
                {
                    newUnreadMessages.Add(new UnreadMessages
                    {
                        UserId = otherUserId,
                        GroupId = groupId,
                        Count = 1
                    });
                }
            }

            if (newUnreadMessages.Count > 0)
            {
                await _context.UnreadMessages.AddRangeAsync(newUnreadMessages);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Group?> ReadGroupAsync(Guid groupId, Guid userId)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(g => g.Id == groupId)
                .Select(group => new Group
                {
                    Id = group.Id,
                    Name = group.Name,
                    ImagePath = group.ImagePath,
                    CreatorId = group.CreatorId,
                    UnreadMessages = group.UnreadMessages
                        .Where(um => um.UserId == userId)
                        .ToList(),
                    Messages = group.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .Take(1)
                        .Select(m => new Message
                        {
                            Content = m.Content,
                            Timestamp = m.Timestamp,
                            Sender = new User
                            {
                                Id = m.Sender.Id,
                                Name = m.Sender.Name
                            }
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeleteUserFromGroupAsync(Guid userId, Guid groupId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return;

            var group = await _context.Groups
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null || !group.Users.Any(u => u.Id == userId) || group.CreatorId == userId)
            {
                return;
            }
            group.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>?> ReadGroupUsersAsync(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            return group?.Users;
        }

        public async Task UpdateNameAsync(Guid groupId, string name)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) return;
            group.Name = name;
            await _context.SaveChangesAsync();
        }
    }
}