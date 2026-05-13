using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.StudentCourses.Events;

public class StudentCourseCreatedEvent(Guid studentId, Guid courseId) : DomainEvent
{
    public Guid StudentId { get; } = studentId;

    public Guid CourseId { get; } = courseId;
}
