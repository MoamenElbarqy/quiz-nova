using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.StudentCourses.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class StudentCourseCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<StudentCourseCreatedEvent>
{
    public async Task Handle(StudentCourseCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses", "students"], ct);
    }
}
