using QuizNova.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
namespace QuizNova.Infrastructure.Services;

public class CacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        // This IS correct because HybridCache has NO GetAsync method
        return await hybridCache.GetOrCreateAsync<T>(
            key,
            _ => new ValueTask<T?>((T?)(object?)null!)!,
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData
            },
            cancellationToken: cancellationToken);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        string[]? tags = null,
        CancellationToken cancellationToken = default)
    {
        await hybridCache.SetAsync(
            key,
            value,
            new HybridCacheEntryOptions { Expiration = expiration },
            tags,
            cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        string[]? tags = null,
        CancellationToken cancellationToken = default)
    {
        return await hybridCache.GetOrCreateAsync<T>(
            key,
            async ct => await factory(ct),
            new HybridCacheEntryOptions { Expiration = expiration },
            tags,
            cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveByTagAsync(tag, cancellationToken);
    }
}
