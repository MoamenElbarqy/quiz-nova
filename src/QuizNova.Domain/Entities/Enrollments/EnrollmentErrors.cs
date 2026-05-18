using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Enrollments;

public static class EnrollmentErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("Enrollment_StudentId_Required", "Student ID is required.");

    public static readonly Error CourseIdRequired =
        Error.Validation("Enrollment_CourseId_Required", "Course ID is required.");

    public static readonly Error EnrollmentDateRequired =
        Error.Validation("Enrollment_EnrollmentDate_Required", "Enrollment date is required.");
}
