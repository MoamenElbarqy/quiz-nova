using System.Diagnostics;

using QuizNova.Application.Common.Interfaces;

using MediatR;

using Microsoft.Extensions.Logging;

namespace QuizNova.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    Stopwatch timer,
    ILogger<TRequest> logger,
    IUser user,
    IAuthService authService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        timer.Start();

        var response = await next();

        timer.Stop();

        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = user.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await authService.GetUserNameAsync(userId);
            }

            logger.LogWarning(
                "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName,
                elapsedMilliseconds,
                userId,
                userName,
                request);
        }

        return response;
    }
}