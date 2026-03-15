using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results.Abstractions;

namespace QuizNova.Application.Common.Behaviours
{
    public class CachingBehavior<TRequest, TResponse>(
        ICashingService cache,
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

            var cachedResult = await cache.GetAsync<TResponse>(
                cachedRequest.CacheKey,
                ct);

            if (cachedResult is not null)
            {
                logger.LogInformation("Cache HIT for {RequestName}", typeof(TRequest).Name);
                return cachedResult;
            }

            var result = await next(ct);

            if (result is IResult { IsSuccess: true })
            {
                logger.LogInformation(
                    "Caching successful result for {RequestName}",
                    typeof(TRequest).Name);

                await cache.SetAsync(
                    cachedRequest.CacheKey,
                    result,
                    cachedRequest.Expiration,
                    cachedRequest.Tags,
                    ct);
            }

            return result;
        }
    }
}
