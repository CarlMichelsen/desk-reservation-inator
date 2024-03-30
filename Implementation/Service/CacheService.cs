using Interface.Service;
using Microsoft.Extensions.Caching.Distributed;

namespace Implementation.Service;

public class CacheService : ICacheService
{
    private readonly IDistributedCache distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public Task<string?> Get(string key)
    {
        return this.distributedCache.GetStringAsync(key);
    }

    public Task Remove(string key)
    {
        return this.distributedCache.RemoveAsync(key);
    }

    public Task Set(string key, string value, TimeSpan ttl)
    {
        var distributedCacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
        };

        return this.distributedCache.SetStringAsync(key, value, distributedCacheEntryOptions);
    }
}
