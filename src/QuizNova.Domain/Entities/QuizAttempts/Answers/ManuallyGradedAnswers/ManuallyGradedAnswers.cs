using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

public abstract class ManuallyGradedAnswers : QuestionAnswer
{
    public int? Score { get; private set; }
    public int MaxMarks { get; private set; }
    public string? Feedback { get; private set; }
    public DateTimeOffset? GradedAt { get; private set; }
    public bool IsGraded => Score.HasValue;

    protected ManuallyGradedAnswers(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        int? score,
        int maxMarks)
        : base(id, studentId, questionId, quizAttemptId)
    {
        Score = score;
        MaxMarks = maxMarks;
    }

    protected ManuallyGradedAnswers()
    {
    }

    public Result<Updated> Grade(int score, string? feedback = null)
    {
        if (IsGraded)
        {
            return ManuallyGradedAnswerErrors.AlreadyGraded;
        }

        if (score < 0)
        {
            return ManuallyGradedAnswerErrors.NegativeScore;
        }

        if (score > MaxMarks)
        {
            return ManuallyGradedAnswerErrors.ScoreExceedsMaxMarks(MaxMarks);
        }

        if (feedback is not null)
        {
            var trimmedFeedback = feedback.Trim();

            if (trimmedFeedback.Length < 3)
            {
                return ManuallyGradedAnswerErrors.FeedbackTooShort;
            }

            if (trimmedFeedback.Length > 200)
            {
                return ManuallyGradedAnswerErrors.FeedbackTooLong;
            }

            Feedback = trimmedFeedback;
        }

        Score = score;
        GradedAt = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    protected void ResetGrading()
    {
        Score = null;
        Feedback = null;
        GradedAt = null;
    }
}
