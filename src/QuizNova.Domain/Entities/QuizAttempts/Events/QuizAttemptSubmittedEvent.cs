using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.QuizAttempts.Events;

public class QuizAttemptSubmittedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
