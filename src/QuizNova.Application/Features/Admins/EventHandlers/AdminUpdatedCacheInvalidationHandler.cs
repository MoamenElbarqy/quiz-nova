using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Admins.Events;

namespace QuizNova.Application.Features.Admins.EventHandlers;

public class AdminUpdatedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<AdminUpdatedEvent>
{
    public async Task Handle(AdminUpdatedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["admins"], ct);
    }
}
