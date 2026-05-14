using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Courses.Events;

public sealed class StudentEnrolledEvent(Guid courseId, Guid studentId) : DomainEvent
{
    public Guid CourseId { get; } = courseId;

    public Guid StudentId { get; } = studentId;
}
