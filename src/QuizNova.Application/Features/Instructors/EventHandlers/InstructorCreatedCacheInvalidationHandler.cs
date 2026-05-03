using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Instructors.Events;

namespace QuizNova.Application.Features.Instructors.EventHandlers;

public class InstructorCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<InstructorCreatedEvent>
{
    public async Task Handle(InstructorCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["instructors"], ct);
    }
}
