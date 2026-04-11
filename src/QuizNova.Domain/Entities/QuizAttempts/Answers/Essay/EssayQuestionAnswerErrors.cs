using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Essay;

public static class EssayQuestionAnswerErrors
{
    public static readonly Error AnswerTextRequired =
        Error.Validation("EssayQuestionAnswer_AnswerText_Required", "Answer text is required.");
}
