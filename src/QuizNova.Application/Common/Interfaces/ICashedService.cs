namespace QuizNova.Application.Common.Interfaces;

public interface ICacheService
{
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        string[]? tags = null,
        CancellationToken cancellationToken = default);
    
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default);
    
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        string[]? tags = null,
        CancellationToken cancellationToken = default);
    
    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);
    
    Task RemoveByTagAsync(
        string tag,
        CancellationToken cancellationToken = default);
}