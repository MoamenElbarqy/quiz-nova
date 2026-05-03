using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Admins.Events;

public class AdminUpdatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
