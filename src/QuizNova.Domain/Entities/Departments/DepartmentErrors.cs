using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Departments;

public static class DepartmentErrors
{
    public static readonly Error CollegeIdRequired =
        Error.Validation("Department_CollegeId_Required", "College ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("Department_Name_Required", "Department name is required.");
}
