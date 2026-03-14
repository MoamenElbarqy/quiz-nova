using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.DepartmentCourses;

public static class DepartmentCourseErrors
{
    public static readonly Error CourseIdRequired =
        Error.Validation("DepartmentCourse_CourseId_Required", "Course ID is required.");

    public static readonly Error DepartmentIdRequired =
        Error.Validation("DepartmentCourse_DepartmentId_Required", "Department ID is required.");
}