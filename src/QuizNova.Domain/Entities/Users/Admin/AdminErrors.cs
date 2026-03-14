using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Users;

public static class AdminErrors
{
    public static readonly Error CollegeIdRequired =
        Error.Validation("Admin_CollegeId_Required", "College ID is required.");
}
