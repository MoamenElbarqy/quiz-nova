using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Admins.Events;

namespace QuizNova.Application.Features.Admins.EventHandlers;

public class AdminCreatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<AdminCreatedEvent>
{
    public async Task Handle(AdminCreatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["admins"], ct);
    }
}
