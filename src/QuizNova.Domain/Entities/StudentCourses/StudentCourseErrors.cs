using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.StudentCourses;

public static class StudentCourseErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("StudentCourse_StudentId_Required", "Student ID is required.");

    public static readonly Error CourseIdRequired =
        Error.Validation("StudentCourse_CourseId_Required", "Course ID is required.");

    public static readonly Error EnrollmentDateRequired =
        Error.Validation("StudentCourse_EnrollmentDate_Required", "Enrollment date is required.");
}
