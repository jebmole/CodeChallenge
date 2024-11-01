using Ease.CodeChallenge.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Ease.CodeChallenge.Infrastructure.Services
{
    public class RedisService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiration);
        }

        public async Task<T> GetCacheValueAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var json = await db.StringGetAsync(key);
            return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
        }
    }
}
