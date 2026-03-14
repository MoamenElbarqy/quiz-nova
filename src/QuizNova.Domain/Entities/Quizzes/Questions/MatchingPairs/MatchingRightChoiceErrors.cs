using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public static class MatchingRightChoiceErrors
{
    public static readonly Error QuestionIdRequired =
        Error.Validation("MatchingRightChoice_QuestionId_Required", "Question ID is required.");

    public static readonly Error ChoiceTextRequired =
        Error.Validation("MatchingRightChoice_ChoiceText_Required", "Choice text is required.");
}
