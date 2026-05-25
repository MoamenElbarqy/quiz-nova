using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;

public static class EssayAnswerErrors
{
    public static readonly Error ResponseTooShort = Error.Validation(
        "EssayAnswer.ResponseTooShort",
        "The student response must be at least 3 characters long.");

    public static readonly Error ResponseTooLong = Error.Validation(
        "EssayAnswer.ResponseTooLong",
        "The student response must not exceed 1000 characters.");

    public static readonly Error ResponseRequired = Error.Validation(
        "EssayAnswer.ResponseRequired",
        "The student response is required.");
}
