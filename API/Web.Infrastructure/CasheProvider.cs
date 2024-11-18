using StackExchange.Redis;
using Web.Application.Interfaces;

namespace Web.Infrastructure
{
    public class CasheProvider : ICasheProvider
    {
        private readonly IDatabase _database;

        public CasheProvider(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<IEnumerable<string>?> GetValuesAsync(string key)
        {
            var values = await _database.SetMembersAsync(key);
            return values.Select(v => v.ToString()).ToList();
        }

        public async Task SetValueAsync(string key, string value)
        {
            await _database.SetAddAsync(key, value);
        }

        public async Task<bool> RemoveValueAsync(string key, string value)
        {
            await _database.SetRemoveAsync(key, value);
            var membersCount = await _database.SetLengthAsync(key);
            if (membersCount == 0)
            {
                await _database.KeyDeleteAsync(key);
                return false;
            }
            return true;
        }
    }
}