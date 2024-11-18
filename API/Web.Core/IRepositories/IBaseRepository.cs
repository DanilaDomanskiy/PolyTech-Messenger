namespace Web.Core.IRepositories
{
    public interface IBaseRepository<Model> where Model : class
    {
        Task<Guid> CreateAsync(Model model);

        Task DeleteAsync(Guid id);

        Task DeleteAsync(Model model);

        Task<Model?> ReadAsync(Guid id);

        Task UpdateAsync(Model model);
    }
}