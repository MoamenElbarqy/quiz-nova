using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

public static class McqQuestionAnswerErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("McqQuestionAnswer_StudentId_Required", "Student ID is required.");

    public static readonly Error QuestionIdRequired =
        Error.Validation("McqQuestionAnswer_QuestionId_Required", "Question ID is required.");

    public static readonly Error SelectedChoiceIdRequired =
        Error.Validation("McqQuestionAnswer_SelectedChoiceId_Required", "Selected choice ID is required.");
}
