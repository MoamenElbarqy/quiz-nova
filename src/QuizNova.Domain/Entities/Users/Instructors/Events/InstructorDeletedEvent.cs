using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Users.Instructors.Events;

public class InstructorDeletedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
