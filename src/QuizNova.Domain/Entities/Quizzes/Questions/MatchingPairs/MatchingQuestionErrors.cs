using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public static class MatchingQuestionErrors
{
    public static readonly Error QuizIdRequired =
        Error.Validation("MatchingQuestion_QuizId_Required", "Quiz ID is required.");

    public static readonly Error PromptRequired =
        Error.Validation("MatchingQuestion_Prompt_Required", "Question prompt is required.");

    public static readonly Error DisplayOrderInvalid =
        Error.Validation("MatchingQuestion_DisplayOrder_Invalid", "Display order must be non-negative.");

    public static readonly Error PointsInvalid =
        Error.Validation("MatchingQuestion_Points_Invalid", "Points must be greater than zero.");
}
