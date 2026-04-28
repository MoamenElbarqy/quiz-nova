using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

public static class QuestionAnswerErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("QuestionAnswer_StudentId_Required", "Student ID is required.");

    public static readonly Error QuestionIdRequired =
        Error.Validation("QuestionAnswer_QuestionId_Required", "Question ID is required.");

    public static readonly Error QuizAttemptIdRequired =
        Error.Validation("QuestionAnswer_QuizAttemptId_Required", "Quiz attempt ID is required.");

    public static readonly Error SelectedChoiceIdRequired =
        Error.Validation("QuestionAnswer_SelectedChoiceId_Required", "Selected choice ID is required.");
}
