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
            var groupAndUser = await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => new
                {
                    Group = g,
                    UserExists = g.Users.Any(u => u.Id == userId),
                    User = _context.Users.FirstOrDefault(u => u.Id == userId)
                })
                .FirstOrDefaultAsync();

            if (groupAndUser != null && !groupAndUser.UserExists && groupAndUser.User != null)
            {
                groupAndUser.Group.Users.Add(groupAndUser.User);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Group>?> GetGroupsAsync(Guid userId)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(g => g.Users.Any(user => user.Id == userId))
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
    }
}