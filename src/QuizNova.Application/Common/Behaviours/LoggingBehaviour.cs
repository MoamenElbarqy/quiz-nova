using MediatR.Pipeline;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;

namespace QuizNova.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger, IUser user, IAuthService authService)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var userId = user.Id ?? string.Empty; // this is will exist only if he the user logged in
        var userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await authService.GetUserNameAsync(userId);
        }

        logger.LogInformation(
            "Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
    }
}