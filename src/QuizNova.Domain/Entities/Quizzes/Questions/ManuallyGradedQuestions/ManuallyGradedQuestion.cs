using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public abstract class ManuallyGradedQuestion : Question
{
    public int? Score { get; private set; }

    public Result<Updated> SetScore(int score)
    {
        if (score < 0)
        {
            return ManuallyGradedQuestionError.NegativeScore;
        }

        if (score > Marks)
        {
            return ManuallyGradedQuestionError.ScoreExceedsMarks;
        }

        Score = score;

        return Result.Updated;
    }

    // Requried By Ef core
    protected ManuallyGradedQuestion()
    {
    }

    protected ManuallyGradedQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }
}
