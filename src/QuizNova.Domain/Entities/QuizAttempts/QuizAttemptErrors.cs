using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts;

public static class QuizAttemptErrors
{
    public static readonly Error AttemptIdRequired =
        Error.Validation("QuizAttempt_Id_Required", "Quiz attempt ID is required.");

    public static readonly Error StudentIdRequired =
        Error.Validation("QuizAttempt_StudentId_Required", "Student ID is required.");

    public static readonly Error QuizIdRequired =
        Error.Validation("QuizAttempt_QuizId_Required", "Quiz ID is required.");

    public static readonly Error StartedAtRequired =
        Error.Validation("QuizAttempt_StartedAt_Required", "Start time is required.");

    public static readonly Error SubmittedAtRequired =
        Error.Validation("QuizAttempt_SubmittedAt_Required", "Submission time is required.");

    public static readonly Error SubmittedAtInvalid =
        Error.Validation("QuizAttempt_SubmittedAt_Invalid", "Submission time cannot be earlier than start time.");

    public static readonly Error AttemptAlreadyCompleted =
        Error.Validation("QuizAttempt_Already_Completed", "Cannot modify a completed attempt.");

    public static Error SubmittedAtAfterQuizEnd(DateTimeOffset quizEndTimeUtc) =>
        Error.Validation(
            "QuizAttempt_SubmittedAt_AfterQuizEnd",
            $"Submission time must be before or equal to quiz end time '{quizEndTimeUtc:O}'.");

    public static Error StartedAtBeforeQuizStart(DateTimeOffset quizStartTimeUtc) =>
        Error.Validation(
            "QuizAttempt_StartedAt_BeforeQuizStart",
            $"Start time must be after or equal to quiz start time '{quizStartTimeUtc:O}'.");

    public static Error StartedAtAfterQuizEnd(DateTimeOffset quizEndTimeUtc) =>
        Error.Validation(
            "QuizAttempt_StartedAt_AfterQuizEnd",
            $"Start time must be before or equal to quiz end time '{quizEndTimeUtc:O}'.");

    public static Error QuestionNotFoundInQuiz(Guid questionId, Guid quizId) =>
        Error.Validation(
            "QuizAttempt_Question_NotFoundInQuiz",
            $"Question '{questionId}' does not belong to quiz '{quizId}'.");
}
