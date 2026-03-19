using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;

public static class MatchingLeftChoiceErrors
{
    public static readonly Error QuestionIdRequired =
        Error.Validation("MatchingLeftChoice_QuestionId_Required", "Question ID is required.");

    public static readonly Error ChoiceTextRequired =
        Error.Validation("MatchingLeftChoice_ChoiceText_Required", "Choice text is required.");
}
