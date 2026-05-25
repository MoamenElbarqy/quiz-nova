using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public static class EssayErrors
{
    public static readonly Error AnswerReferenceRequired =
        Error.Validation("Essay_AnswerReference_Required", "Answer reference cannot be empty when provided.");

    public static readonly Error AnswerReferenceTooShort =
        Error.Validation("Essay_AnswerReference_TooShort", "Answer reference must be at least 3 characters long.");

    public static readonly Error AnswerReferenceTooLong =
        Error.Validation("Essay_AnswerReference_TooLong", "Answer reference must not exceed 1000 characters.");
}
