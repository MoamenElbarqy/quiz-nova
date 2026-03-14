using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts;

public static class QuizAttemptErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("QuizAttempt_StudentId_Required", "Student ID is required.");

    public static readonly Error QuizIdRequired =
        Error.Validation("QuizAttempt_QuizId_Required", "Quiz ID is required.");

    public static readonly Error StartedAtRequired =
        Error.Validation("QuizAttempt_StartedAt_Required", "Start time is required.");

    public static readonly Error SubmittedAtInvalid =
        Error.Validation("QuizAttempt_SubmittedAt_Invalid", "Submission time cannot be earlier than start time.");
}
