using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Quizzes.Events;

namespace QuizNova.Application.Features.Quizzes.EventHandlers;

public class QuizCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<QuizCreatedEvent>
{
    public async Task Handle(QuizCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["quizzes"], ct);
    }
}
