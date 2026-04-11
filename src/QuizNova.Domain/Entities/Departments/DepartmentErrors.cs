using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Departments;

public static class DepartmentErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Department_Name_Required", "Department name is required.");
}
