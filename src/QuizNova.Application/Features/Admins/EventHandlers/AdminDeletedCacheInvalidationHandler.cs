using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Entities.Users.Admins.Events;

namespace QuizNova.Application.Features.Admins.EventHandlers;

public class AdminDeletedCacheInvalidationHandler(ICacheInvalidator cacheInvalidator)
    : INotificationHandler<AdminDeletedEvent>
{
    public async Task Handle(AdminDeletedEvent notification, CancellationToken ct)
    {
        await cacheInvalidator.InvalidateAsync(["admins"], ct);
    }
}
