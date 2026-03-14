using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Essay;

public static class EssayQuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("EssayQuestion_QuizId_Required", "Quiz ID is required.");

    public static readonly Error PromptRequired =
        Error.Validation("EssayQuestion_Prompt_Required", "Question prompt is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("EssayQuestion_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error PointsInvalid =
        Error.Validation("EssayQuestion_Points_Invalid", "Points must be greater than zero.");
}
