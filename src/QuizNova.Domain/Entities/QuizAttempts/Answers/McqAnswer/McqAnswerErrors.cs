using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;

public static class McqAnswerErrors
{
    public static readonly Error SelectedChoiceIdRequired =
        Error.Validation("McqAnswer_SelectedChoiceId_Required", "Selected choice ID is required.");

    public static Error QuestionMismatch(Guid questionId, Guid expectedQuestionId) =>
        Error.Validation(
            "McqAnswer_Question_Mismatch",
            $"Question '{questionId}' does not match MCQ question '{expectedQuestionId}'.");

    public static Error SelectedChoiceDoesNotBelongToQuestion(Guid questionId, Guid selectedChoiceId) =>
        Error.Validation(
            "McqAnswer_SelectedChoice_Invalid",
            $"Selected choice '{selectedChoiceId}' does not belong to question '{questionId}'.");
}
