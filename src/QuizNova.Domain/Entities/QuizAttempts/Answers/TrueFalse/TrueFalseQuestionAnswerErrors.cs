using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;

public static class TrueFalseQuestionAnswerErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("TrueFalseQuestionAnswer_StudentId_Required", "Student ID is required.");

    public static readonly Error QuestionIdRequired =
        Error.Validation("TrueFalseQuestionAnswer_QuestionId_Required", "Question ID is required.");
}
