using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public static class MatchingCorrectPairErrors
{
    public static readonly Error QuestionIdRequired =
        Error.Validation("MatchingCorrectPair_QuestionId_Required", "Question ID is required.");

    public static readonly Error LeftChoiceIdRequired =
        Error.Validation("MatchingCorrectPair_LeftChoiceId_Required", "Left choice ID is required.");

    public static readonly Error RightChoiceIdRequired =
        Error.Validation("MatchingCorrectPair_RightChoiceId_Required", "Right choice ID is required.");
}
