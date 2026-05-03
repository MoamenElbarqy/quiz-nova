using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Student.Events;

namespace QuizNova.Application.Features.Students.EventHandlers;

public class StudentDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<StudentDeletedEvent>
{
    public async Task Handle(StudentDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["students", $"students:{notification.Id}"], ct);
    }
}
