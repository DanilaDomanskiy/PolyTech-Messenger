using Web.Core.IRepositories;

namespace Web.Persistence.Repositories
{
    public abstract class BaseRepository<Model> : IBaseRepository<Model> where Model : class
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
    }
}
