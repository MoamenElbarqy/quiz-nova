using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.StudentCourses.Events;

namespace QuizNova.Application.Features.Courses.EventHandlers;

public class StudentCourseDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<StudentCourseDeletedEvent>
{
    public async Task Handle(StudentCourseDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["courses", "students"], ct);
    }
}
