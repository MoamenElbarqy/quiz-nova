using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.Events;

namespace QuizNova.Application.Features.QuizAttempts.EventHandlers;

public sealed class QuestionGradedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<QuestionGradedEvent>
{
    public async Task Handle(QuestionGradedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["quiz-attempts"], ct);
    }
}
