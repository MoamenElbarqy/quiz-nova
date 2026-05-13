using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class CourseUpdatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<CourseUpdatedEvent>
{
    public async Task Handle(CourseUpdatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses"], ct);
    }
}
