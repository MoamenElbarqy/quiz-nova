using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Hybrid;

using QuizNova.Application.Common.Interfaces;

namespace QuizNova.Infrastructure.Caching;

public class CacheInvalidator(HybridCache hybridCache, IOutputCacheStore outputCacheStore) : ICacheInvalidator
{
    public async Task InvalidateAsync(string[] tags, CancellationToken ct = default)
    {
        var removeHybridCacheTask = hybridCache.RemoveByTagAsync(tags, ct).AsTask();
        var evictOutputCacheTask = Task.WhenAll(tags.Select(tag => outputCacheStore.EvictByTagAsync(tag, ct).AsTask()));

        await Task.WhenAll(removeHybridCacheTask, evictOutputCacheTask);
    }
}
