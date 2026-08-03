namespace QuizNova.Application.Common.Events;

using QuizNova.Domain.Common;

public interface IDomainEventTracker
{
    void TrackEntity(Entity entity);

    IReadOnlyCollection<DomainEvent> GetAndClearDomainEvents();
}
