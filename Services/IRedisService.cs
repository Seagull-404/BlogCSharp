
    using System.Text.Json;
    namespace BlogCSharp.Services{
    public interface IRedisService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expires = null);
        Task DeleteAsync(string key);

        Task<bool> ExistsAsync(string key);
    }
    }
   