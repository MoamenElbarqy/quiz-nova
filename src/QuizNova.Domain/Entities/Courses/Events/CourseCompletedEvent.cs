using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Courses.Events;

public class CourseCompletedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
