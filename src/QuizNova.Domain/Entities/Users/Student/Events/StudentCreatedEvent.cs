using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Student.Events;

public class StudentCreatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
