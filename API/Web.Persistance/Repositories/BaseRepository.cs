using Microsoft.EntityFrameworkCore;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public abstract class BaseRepository<Model> : IBaseRepository<Model> where Model : class, IEntity
    {
        protected readonly WebContext _context;

        protected BaseRepository(WebContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Model model)
        {
            await _context.Set<Model>().AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<Model?> ReadAsync(int id)
        {
            return await _context.Set<Model>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
