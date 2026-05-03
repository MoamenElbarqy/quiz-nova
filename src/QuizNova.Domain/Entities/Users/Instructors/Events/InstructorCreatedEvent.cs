using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Instructors.Events;

public class InstructorCreatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
