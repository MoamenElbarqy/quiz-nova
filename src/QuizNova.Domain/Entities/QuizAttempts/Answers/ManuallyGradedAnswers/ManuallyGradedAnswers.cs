using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

public abstract class ManuallyGradedAnswers : QuestionAnswer
{
    public int? Score { get; private set; }

    protected ManuallyGradedAnswers(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        int? score)
        : base(id, studentId, questionId, quizAttemptId)
    {
        Score = score;
    }

    protected ManuallyGradedAnswers()
    {
    }

    public void UpdateMarks(int? score)
    {
        Score = score;
    }
}
