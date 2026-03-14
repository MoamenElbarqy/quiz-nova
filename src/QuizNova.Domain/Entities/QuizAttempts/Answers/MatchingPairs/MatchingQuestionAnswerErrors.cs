using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.MatchingPairs;

public static class MatchingQuestionAnswerErrors
{
    public static readonly Error StudentIdRequired =
        Error.Validation("MatchingQuestionAnswer_StudentId_Required", "Student ID is required.");

    public static readonly Error QuestionIdRequired =
        Error.Validation("MatchingQuestionAnswer_QuestionId_Required", "Question ID is required.");

    public static readonly Error LeftChoiceIdRequired =
        Error.Validation("MatchingQuestionAnswer_LeftChoiceId_Required", "Left choice ID is required.");

    public static readonly Error RightChoiceIdRequired =
        Error.Validation("MatchingQuestionAnswer_RightChoiceId_Required", "Right choice ID is required.");
}
