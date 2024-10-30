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

        public async Task<Guid> CreateAsync(Model model)
        {
            await _context.Set<Model>().AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public virtual async Task<Model?> ReadAsync(Guid id)
        {
            return await _context.Set<Model>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Model model)
        {
            var existingModel = await _context.Set<Model>().FindAsync(model.Id);
            if (existingModel != null)
            {
                _context.Entry(existingModel).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync();
            }
        }
    }
}