using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Courses.Events;

public class CourseDeletedEvent(Guid id) : DomainEvent
{
    public Guid Id { get; } = id;
}
