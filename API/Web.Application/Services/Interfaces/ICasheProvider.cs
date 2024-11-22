namespace Web.Application.Services.Interfaces
{
    public interface ICasheProvider
    {
        Task<IEnumerable<string>?> GetValuesAsync(string key);

        Task<bool> RemoveValueAsync(string key, string value);

        Task SetValueAsync(string key, string value);
    }
}