using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Colleges;

public static class CollegeErrors
{
    public static readonly Error NameRequired =
        Error.Validation("College_Name_Required", "College name is required.");
}
