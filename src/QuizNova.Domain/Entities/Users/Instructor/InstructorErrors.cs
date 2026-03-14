using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users;

public static class InstructorErrors
{
    public static readonly Error CollegeIdRequired =
        Error.Validation("Instructor_CollegeId_Required", "College ID is required.");
}
