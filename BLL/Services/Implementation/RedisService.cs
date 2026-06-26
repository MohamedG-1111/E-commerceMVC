using System.Text.Json;
using E_commerce.BLL.Services.Interfaces;
using StackExchange.Redis;

namespace E_commerce.BLL.Services.Implementation
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _dbRedis;

        public RedisService(IConnectionMultiplexer dbRedis)
        {
            _dbRedis = dbRedis.GetDatabase();
        }

        public async Task<T?> SetAsync<T>(string key, T value, TimeSpan? TTL = null)
        {
            var json = JsonSerializer.Serialize<T>(value);
            await _dbRedis.StringSetAsync(key, json, TTL ?? TimeSpan.FromDays(7));
            return await GetAsync<T?>(key);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _dbRedis.KeyDeleteAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _dbRedis.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(json.ToString()!);
        }
    }
}