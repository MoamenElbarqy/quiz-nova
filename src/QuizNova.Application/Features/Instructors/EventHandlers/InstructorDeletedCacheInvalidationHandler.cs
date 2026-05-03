using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Instructors.Events;

namespace QuizNova.Application.Features.Instructors.EventHandlers;

public class InstructorDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<InstructorDeletedEvent>
{
    public async Task Handle(InstructorDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["instructors"], ct);
    }
}
