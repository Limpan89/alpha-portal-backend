using Microsoft.Extensions.Caching.Memory;

namespace Business.Handlers;

public interface ICacheHandler<T>
{
    T? Get(string key);
    T Set(string key, T data, int minutesCache = 30);
}

public class CacheHandler<T>(IMemoryCache cache) : ICacheHandler<T>
{
    private readonly IMemoryCache _cache = cache;

    public T? Get(string key)
    {
        return _cache.TryGetValue(key, out T? data) ? data : default;
    }

    public T Set(string key, T data, int minutesCache = 30)
    {
        _cache.Remove(key);
        _cache.Set(key, data, TimeSpan.FromMinutes(minutesCache));
        return data;
    }
}
