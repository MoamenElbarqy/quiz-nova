using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.QuizAttempts.Events;

namespace QuizNova.Application.Features.QuizAttempts.EventHandlers;

public class QuizAttemptSubmittedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<QuizAttemptSubmittedEvent>
{
    public async Task Handle(QuizAttemptSubmittedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["quiz-attempts", "quizzes"], ct);
    }
}
