using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public static class QuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("Question_QuizId_Required", "Quiz ID is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("Question_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error PointsInvalid =
        Error.Validation("Question_Points_Invalid", "Points must be greater than zero.");
}
