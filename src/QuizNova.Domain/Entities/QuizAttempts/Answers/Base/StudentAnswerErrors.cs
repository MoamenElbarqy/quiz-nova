using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

public static class StudentAnswerErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("StudentAnswer_StudentId_Required", "Student ID is required.");

    public static readonly Error QuestionIdRequired =
        Error.Validation("StudentAnswer_QuestionId_Required", "Question ID is required.");

    public static readonly Error QuizAttemptIdRequired =
        Error.Validation("StudentAnswer_QuizAttemptId_Required", "Quiz attempt ID is required.");

    public static readonly Error SelectedChoiceIdRequired =
        Error.Validation("StudentAnswer_SelectedChoiceId_Required", "Selected choice ID is required.");
}
