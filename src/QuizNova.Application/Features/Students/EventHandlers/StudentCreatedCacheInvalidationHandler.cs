using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Student.Events;

namespace QuizNova.Application.Features.Students.EventHandlers;

public class StudentCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<StudentCreatedEvent>
{
    public async Task Handle(StudentCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["students"], ct);
    }
}
