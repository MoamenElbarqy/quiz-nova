using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Student.Events;

public class StudentUpdatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
