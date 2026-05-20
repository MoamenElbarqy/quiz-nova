using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public static class EssayErrors
{
    public static readonly Error AnswerReferenceRequired =
        Error.Validation("Essay_AnswerReference_Required", "Answer reference cannot be empty when provided.");
}
