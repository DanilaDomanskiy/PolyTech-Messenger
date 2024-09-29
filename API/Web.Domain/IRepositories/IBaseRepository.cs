namespace Web.Core.IRepositories
{
    public interface IBaseRepository<Model> where Model : class
    {
        Task CreateAsync(Model model);
    }
}