using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

public static class McqQuestionAnswerErrors
{
    public static readonly Error SelectedChoiceIdRequired =
        Error.Validation("McqQuestionAnswer_SelectedChoiceId_Required", "Selected choice ID is required.");
}
