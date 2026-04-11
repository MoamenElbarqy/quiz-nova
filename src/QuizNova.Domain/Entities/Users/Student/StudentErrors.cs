using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users;

public static class StudentErrors
{
    public static readonly Error DepartmentIdRequired =
        Error.Validation("Student_DepartmentId_Required", "Department ID is required.");

    public static readonly Error LevelIdRequired =
        Error.Validation("Student_LevelId_Required", "Level ID is required.");
}
