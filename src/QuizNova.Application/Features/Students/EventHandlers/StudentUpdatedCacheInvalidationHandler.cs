using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Student.Events;

namespace QuizNova.Application.Features.Students.EventHandlers;

public class StudentUpdatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<StudentUpdatedEvent>
{
    public async Task Handle(StudentUpdatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["students"], ct);
    }
}
