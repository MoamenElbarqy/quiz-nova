using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

public static class McqQuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("McqQuestion_QuizId_Required", "Quiz ID is required.");

    public static readonly Error PromptRequired =
        Error.Validation("McqQuestion_Prompt_Required", "Question prompt is required.");

    public static readonly Error NumberOfChoicesInvalid =
        Error.Validation("McqQuestion_NumberOfChoices_Invalid", "Number of choices must be at least 2.");

    public static readonly Error CorrectChoiceIdRequired =
        Error.Validation("McqQuestion_CorrectChoiceId_Required", "Correct choice ID is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("McqQuestion_DisplayOrder_Invalid", "Display order cannot be negative.");

    public static readonly Error PointsInvalid =
        Error.Validation("McqQuestion_Points_Invalid", "Points must be greater than zero.");
}
