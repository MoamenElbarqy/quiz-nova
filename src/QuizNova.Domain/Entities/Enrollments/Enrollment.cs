using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Enrollments.Events;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.Enrollments;

public class Enrollment : Entity
{
    public Guid StudentId { get; }

    public Guid CourseId { get; }

    public DateTimeOffset EnrolledOnUtc { get; private set; }

    public Student? Student { get; init; }

    public Course? Course { get; init; }

    private Enrollment()
    {
    }

    private Enrollment(Guid id, Guid studentId, Guid courseId, DateTimeOffset enrolledOnUtc)
        : base(id)
    {
        StudentId = studentId;
        CourseId = courseId;
        EnrolledOnUtc = enrolledOnUtc;
    }

    public static Result<Enrollment> Create(Guid id, Guid studentId, Guid courseId, DateTimeOffset enrolledOnUtc)
    {
        if (studentId == Guid.Empty)
        {
            return EnrollmentErrors.StudentIdRequired;
        }

        if (courseId == Guid.Empty)
        {
            return EnrollmentErrors.CourseIdRequired;
        }

        if (enrolledOnUtc == default)
        {
            return EnrollmentErrors.EnrollmentDateRequired;
        }

        var enrollment = new Enrollment(id, studentId, courseId, enrolledOnUtc);
        enrollment.AddDomainEvent(new EnrollmentCreatedEvent(studentId, courseId));
        return enrollment;
    }

    public Result<Deleted> Delete()
    {
        AddDomainEvent(new EnrollmentDeletedEvent(StudentId, CourseId));
        return Result.Deleted;
    }
}
