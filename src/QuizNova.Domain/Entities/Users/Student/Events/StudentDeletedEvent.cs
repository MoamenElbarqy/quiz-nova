using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Student.Events;

public class StudentDeletedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
