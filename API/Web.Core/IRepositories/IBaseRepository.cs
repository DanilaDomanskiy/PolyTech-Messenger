namespace Web.Core.IRepositories
{
    public interface IBaseRepository<Model> where Model : class
    {
        Task CreateAsync(Model model);

        Task<Model?> ReadAsync(int id);

        Task UpdateAsync(Model model);
    }
}