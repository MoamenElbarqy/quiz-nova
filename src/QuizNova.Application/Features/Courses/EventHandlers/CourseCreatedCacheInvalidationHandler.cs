using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Courses.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class CourseCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<CourseCreatedEvent>
{
    public async Task Handle(CourseCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses"], ct);
    }
}
