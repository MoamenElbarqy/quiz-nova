using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Courses.Events;

public class CourseUpdatedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
