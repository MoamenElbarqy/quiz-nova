using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class CourseDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<CourseDeletedEvent>
{
    public async Task Handle(CourseDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses"], ct);
    }
}
