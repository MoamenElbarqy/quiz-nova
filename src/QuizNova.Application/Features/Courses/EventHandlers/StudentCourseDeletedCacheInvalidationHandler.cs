using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Enrollments.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class EnrollmentDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<EnrollmentDeletedEvent>
{
    public async Task Handle(EnrollmentDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses", "students"], ct);
    }
}
