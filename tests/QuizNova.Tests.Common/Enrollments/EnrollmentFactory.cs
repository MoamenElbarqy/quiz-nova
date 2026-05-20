using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Enrollments;

namespace QuizNova.Tests.Common.Enrollments;

public static class EnrollmentFactory
{
    public static Result<Enrollment> CreateEnrollment(
        Guid? id = null,
        Guid? studentId = null,
        Guid? courseId = null,
        DateTimeOffset? enrolledOnUtc = null)
    {
        return Enrollment.Create(
            id ?? Guid.NewGuid(),
            studentId ?? Guid.NewGuid(),
            courseId ?? Guid.NewGuid(),
            enrolledOnUtc ?? DateTimeOffset.UtcNow);
    }
}
