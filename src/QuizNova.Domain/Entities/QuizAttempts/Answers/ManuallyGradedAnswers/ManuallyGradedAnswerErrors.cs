using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

public static class ManuallyGradedAnswerErrors
{
    public static readonly Error AlreadyGraded =
        Error.Conflict("ManuallyGradedAnswer.AlreadyGraded", "This answer has already been graded.");

    public static readonly Error NegativeScore =
        Error.Validation("ManuallyGradedAnswer.Score.Negative", "Score cannot be negative.");

    public static Error ScoreExceedsMaxMarks(int maxMarks) =>
        Error.Validation(
            "ManuallyGradedAnswer.Score.ExceedsMaxMarks",
            $"Score cannot exceed the question's maximum marks of {maxMarks}.");

    public static readonly Error FeedbackTooShort =
        Error.Validation(
            "ManuallyGradedAnswer.Feedback.TooShort",
            "Feedback must be at least 3 characters long.");

    public static readonly Error FeedbackTooLong =
        Error.Validation(
            "ManuallyGradedAnswer.Feedback.TooLong",
            "Feedback must not exceed 200 characters.");
}
