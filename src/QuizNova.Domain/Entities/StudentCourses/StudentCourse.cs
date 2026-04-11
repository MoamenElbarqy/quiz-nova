using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.StudentCourses;

public class StudentCourse : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public DateTimeOffset EnrolledOnUtc { get; private set; }

    public Student? Student { get; private set; }

    public Course? Course { get; private set; }

    private StudentCourse()
    {
    }

    private StudentCourse(Guid id, Guid studentId, Guid courseId, DateTimeOffset enrolledOnUtc)
        : base(id)
    {
        StudentId = studentId;
        CourseId = courseId;
        EnrolledOnUtc = enrolledOnUtc;
    }

    public static Result<StudentCourse> Create(Guid id, Guid studentId, Guid courseId, DateTimeOffset enrolledOnUtc)
    {
        if (studentId == Guid.Empty)
        {
            return StudentCourseErrors.StudentIdRequired;
        }

        if (courseId == Guid.Empty)
        {
            return StudentCourseErrors.CourseIdRequired;
        }

        if (enrolledOnUtc == default)
        {
            return StudentCourseErrors.EnrollmentDateRequired;
        }

        return new StudentCourse(id, studentId, courseId, enrolledOnUtc);
    }
}
