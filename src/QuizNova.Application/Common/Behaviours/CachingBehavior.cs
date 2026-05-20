using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results.Abstractions;

namespace QuizNova.Application.Common.Behaviours;

public class CachingBehavior<TRequest, TResponse>(
    HybridCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ICachedQuery cachedRequest)
        {
            return await next(ct);
        }

        logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);

        var result = await cache.GetOrCreateAsync<TResponse>(
            cachedRequest.CacheKey,
            _ => new ValueTask<TResponse>((TResponse)(object)null!),
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableUnderlyingData,
            },
            cancellationToken: ct);

        if (result is null)
        {
            result = await next(ct);

            if (result is not IResult { IsSuccess: true })
            {
                return result;
            }

            logger.LogInformation("Caching result for {RequestName}", typeof(TRequest).Name);

            await cache.SetAsync(
                cachedRequest.CacheKey,
                result,
                new HybridCacheEntryOptions
                {
                    Expiration = cachedRequest.Expiration,
                },
                cachedRequest.Tags,
                ct);
        }

        return result;
    }
}