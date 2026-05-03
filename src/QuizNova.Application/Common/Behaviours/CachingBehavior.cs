using MediatR;

using Microsoft.Extensions.Caching.Hybrid;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results.Abstractions;

namespace QuizNova.Application.Common.Behaviours;

public class CachingBehavior<TRequest, TResponse>(HybridCache hybridCache)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : IResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
#pragma warning disable EXTEXP0018
        return await hybridCache.GetOrCreateAsync(
            request.CacheKey,
            async token => await next(token),
            new HybridCacheEntryOptions { Expiration = request.Expiration },
            request.Tags,
            ct);
#pragma warning restore EXTEXP0018
    }
}
