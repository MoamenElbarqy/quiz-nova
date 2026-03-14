using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

public static class TrueFalseQuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("TrueFalseQuestion_QuizId_Required", "Quiz ID is required.");

    public static readonly Error StatementRequired =
        Error.Validation("TrueFalseQuestion_Statement_Required", "Question statement is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("TrueFalseQuestion_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error PointsInvalid =
        Error.Validation("TrueFalseQuestion_Points_Invalid", "Points must be greater than zero.");
}
