using HackerNews.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace HackerNews.Infrastructure.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!);
            _database = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }
    }
}
