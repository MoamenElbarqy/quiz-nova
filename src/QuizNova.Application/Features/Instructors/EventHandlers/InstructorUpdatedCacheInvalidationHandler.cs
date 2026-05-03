using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Instructors.Events;

namespace QuizNova.Application.Features.Instructors.EventHandlers;

public class InstructorUpdatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<InstructorUpdatedEvent>
{
    public async Task Handle(InstructorUpdatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["instructors"], ct);
    }
}
