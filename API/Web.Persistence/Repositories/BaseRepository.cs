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

        public virtual async Task<Guid> CreateAsync(Model model)
        {
            await _context.Set<Model>().AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public virtual async Task<Model?> ReadAsync(Guid id)
        {
            return await _context.Set<Model>().FindAsync(id);
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

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Set<Model>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Model>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Model model)
        {
            _context.Set<Model>().Remove(model);
            await _context.SaveChangesAsync();
        }
    }
}