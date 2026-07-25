using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results.Abstractions;

namespace QuizNova.Application.Common.Behaviours;

public class CachingBehavior<TRequest, TResponse>(
    HybridCache cache,
    IOptions<CacheSettings> cacheSettings,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly TimeSpan _queryCacheDuration = TimeSpan.FromMinutes(cacheSettings.Value.QueryCacheDurationMinutes);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ICachedQuery cachedRequest)
        {
            return await next(ct);
        }

        var key = cachedRequest.CacheKey;

        logger.LogInformation("Checking cache for {RequestName} with key {CacheKey}", typeof(TRequest).Name, key);

        var result = await cache.GetOrCreateAsync<TResponse>(
            key,
            _ => new ValueTask<TResponse>((TResponse)(object)null!),
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData,
            },
            cachedRequest.Tags,
            ct);

        if (result is null)
        {
            result = await next(ct);

            if (result is not IResult { IsSuccess: true })
            {
                return result;
            }

            logger.LogInformation("Caching result for {RequestName} with key {CacheKey}", typeof(TRequest).Name, key);

            await cache.SetAsync(
                key,
                result,
                new HybridCacheEntryOptions
                {
                    Expiration = _queryCacheDuration,
                },
                cachedRequest.Tags,
                ct);
        }

        return result;
    }
}
