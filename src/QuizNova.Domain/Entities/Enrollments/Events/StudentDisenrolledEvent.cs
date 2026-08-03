using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Enrollments.Events;

public class StudentDisenrolledEvent(Guid studentId, Guid courseId) : DomainEvent
{
    public Guid StudentId { get; } = studentId;

    public Guid CourseId { get; } = courseId;
}
