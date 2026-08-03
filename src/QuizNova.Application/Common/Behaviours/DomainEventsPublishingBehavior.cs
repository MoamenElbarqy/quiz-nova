namespace QuizNova.Application.Common.Behaviours;

using System.Text.Json;

using Events;

using Interfaces;

using MediatR;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results.Abstractions;

public class DomainEventsPublishingBehavior<TRequest, TResponse>(
    IDomainEventTracker eventTracker,
    IAppDbContext appDbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next(ct);

        if (!response.IsSuccess)
        {
            return response;
        }

        var trackedEvents = eventTracker.GetAndClearDomainEvents();
        if (trackedEvents.Count != 0)
        {
            var outboxMessages = trackedEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
            }).ToList();

            await appDbContext.OutboxMessages.AddRangeAsync(outboxMessages, ct);
            await appDbContext.SaveChangesAsync(ct);
        }

        return response;
    }
}
