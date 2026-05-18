using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Enrollments.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class EnrollmentCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<EnrollmentCreatedEvent>
{
    public async Task Handle(EnrollmentCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses", "students"], ct);
    }
}
