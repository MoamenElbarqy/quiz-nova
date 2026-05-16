using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public static class ManuallyGradedQuestionError
{
    public static readonly Error NegativeScore =
        Error.Validation("ManuallyGradedQuestion_Score_Negative", "Score cannot be negative.");

    public static readonly Error ScoreExceedsMarks =
        Error.Validation("ManuallyGradedQuestion_Score_ExceedsMarks", "Score cannot exceed maximum marks for the question.");
}
