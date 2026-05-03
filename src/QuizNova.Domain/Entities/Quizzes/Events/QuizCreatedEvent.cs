using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Quizzes.Events;

public class QuizCreatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
