using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

public class ManuallyGradedAnswers : QuestionAnswer
{
    public int? Score { get; private set; }

    public ManuallyGradedAnswers(
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
