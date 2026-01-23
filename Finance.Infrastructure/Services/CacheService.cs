using Finance.Contracts.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Finance.Infrastructure.Services;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    private readonly DistributedCacheEntryOptions _defaultOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedValue = await distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedValue))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
    {
        var options = expirationTime.HasValue
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime.Value }
            : _defaultOptions;

        var jsonValue = JsonSerializer.Serialize(value);

        await distributedCache.SetStringAsync(key, jsonValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        await distributedCache.RemoveAsync(key);
    }
}