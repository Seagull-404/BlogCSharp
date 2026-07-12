using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace BlogCSharp.Services
{
    public class RedisService : IRedisService
    {
       private readonly IDatabase _database;
       public RedisService(IConnectionMultiplexer redis)
       {
        _database = redis.GetDatabase();
       }
       
       public async Task<T?> GetAsync<T>(string key)
       {
        var value = await _database.StringGetAsync(key);
        if(!value.HasValue) return default;
        return JsonSerializer.Deserialize<T>(value);
       }

       public async Task SetAsync<T>(string key, T value, TimeSpan? expires = null)
       {
           if (expires.HasValue)
           {
               await _database.StringSetAsync(key, JsonSerializer.Serialize(value), (Expiration)expires.Value);
           }
           else
           {
               await _database.StringSetAsync(key, JsonSerializer.Serialize(value));
           }
       }

       public async Task DeleteAsync(string key)
       {
        await _database.KeyDeleteAsync(key);
       }

       public async Task<bool> ExistsAsync(string key)
       {
        return await _database.KeyExistsAsync(key);
       }
    }
}