﻿using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WebContext context) : base(context)
        {
        }

        public async Task<User?> ReadByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>?> ReadByEmailLettersAsync(string email, Guid userId)
        {
            return await _context.Users
                .Where(u => u.Email.StartsWith(email) && u.Id != userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>?> ReadNoGroupUsersAsync(string email, Guid groupId)
        {
            return await _context.Users
                .Where(u => u.Email.StartsWith(email) && !u.Groups.Any(g => g.Id == groupId))
                .ToListAsync();
        }
    }
}